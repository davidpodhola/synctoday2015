module ExchangeAppointmentsSQL

open Common
open System
open System.Data
open System.Data.SqlClient
open sync.today.Models
open ExchangeCommon
open FSharp.Data

let logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
let standardAttrsVisiblyDifferentLogger = log4net.LogManager.GetLogger( "StandardAttrsVisiblyDifferent" )

type GetExchangeAppointmentsQuery = SqlCommandProvider<"GetExchangeAppointments.sql", ConnectionStringName>
type SaveDLUPIssuesQuery = SqlCommandProvider<"SaveDLUPIssues.sql", ConnectionStringName>
type SaveExchangeAppointmentsQuery = SqlCommandProvider<"SaveExchangeAppointment.sql", ConnectionStringName>

let internal convert( r : GetExchangeAppointmentsQuery.Record ) : ExchangeAppointmentDTO =
    { Id = r.Id; InternalId = r.InternalId; ExternalId = r.ExternalId; Body = r.Body; Start = r.Start; End = r.End; LastModifiedTime = r.LastModifiedTime; Location = r.Location;
                    IsReminderSet = r.IsReminderSet; AppointmentState = r.AppointmentState; Subject = r.Subject; RequiredAttendeesJSON = r.RequiredAttendeesJSON;
                    ReminderMinutesBeforeStart = r.ReminderMinutesBeforeStart; Sensitivity = r.Sensitivity; RecurrenceJSON = r.RecurrenceJSON; ModifiedOccurrencesJSON = r.ModifiedOccurrencesJSON;
                    LastOccurrenceJSON = r.LastOccurrenceJSON; IsRecurring = r.IsRecurring; IsCancelled = r.IsCancelled; ICalRecurrenceId = r.ICalRecurrenceId; 
                    FirstOccurrenceJSON = r.FirstOccurrenceJSON; 
                    DeletedOccurrencesJSON = r.DeletedOccurrencesJSON; AppointmentType = r.AppointmentType; Duration = r.Duration; StartTimeZone = r.StartTimeZone; 
                    EndTimeZone = r.EndTimeZone; AllowNewTimeProposal = r.AllowNewTimeProposal; CategoriesJSON = r.CategoriesJSON; ServiceAccountId = r.ServiceAccountId; 
                    Tag = r.Tag }

let internal convertOption( ro : GetExchangeAppointmentsQuery.Record option) : ExchangeAppointmentDTO option = 
    match ro with
    | Some r -> Some(convert(r))
    | None -> None

let exchangeAppointmentByInternalId internalId = 
    ( new GetExchangeAppointmentsQuery() ).AsyncExecute(0,internalId, null ) |> Async.RunSynchronously |> Seq.tryHead |> convertOption

let exchangeAppointments () = 
    ( new GetExchangeAppointmentsQuery() ).AsyncExecute(0,Guid.Empty, null ) |> Async.RunSynchronously |> Seq.map convert


let saveDLUPIssues( externalId : string, lastDLError : string, lastUPError : string  ) = 
    devlog.Debug( ( sprintf "saveDLUPIssues: externalId:'%A', LastDLError:'%A', LastUPError:'%A'" externalId, lastDLError, lastUPError  ) )
    ( new SaveDLUPIssuesQuery() ).AsyncExecute(lastDLError, lastUPError, externalId ) |> Async.RunSynchronously |> ignore

let categories( r : ExchangeAppointmentDTO ) : string array =
    if optionstringIsEmpty r.CategoriesJSON then [| |] else unjson<string array>( r.CategoriesJSON.Value )
    
let normalize ( r : ExchangeAppointmentDTO ) : ExchangeAppointmentDTO =
    { Id = r.Id; InternalId = r.InternalId; ExternalId = r.ExternalId; Body = Some(optionString2String r.Body); Start = fixDateSecs(r.Start); End = fixDateSecs(r.End); LastModifiedTime = fixDateSecs(r.LastModifiedTime); 
        Location = r.Location;
        IsReminderSet = r.IsReminderSet; AppointmentState = r.AppointmentState; Subject = r.Subject; RequiredAttendeesJSON = r.RequiredAttendeesJSON;
        ReminderMinutesBeforeStart = r.ReminderMinutesBeforeStart; Sensitivity = r.Sensitivity; RecurrenceJSON = r.RecurrenceJSON; ModifiedOccurrencesJSON = r.ModifiedOccurrencesJSON;
        LastOccurrenceJSON = r.LastOccurrenceJSON; IsRecurring = r.IsRecurring; IsCancelled = r.IsCancelled; ICalRecurrenceId = r.ICalRecurrenceId; 
        FirstOccurrenceJSON = r.FirstOccurrenceJSON; 
        DeletedOccurrencesJSON = r.DeletedOccurrencesJSON; AppointmentType = r.AppointmentType; Duration = r.Duration; StartTimeZone = r.StartTimeZone; 
        EndTimeZone = r.EndTimeZone; AllowNewTimeProposal = r.AllowNewTimeProposal; 
        CategoriesJSON = Some(json(Array.FindAll( categories(r), ( fun p -> not(String.IsNullOrWhiteSpace( p ) ) ) ) ) );
        ServiceAccountId = r.ServiceAccountId; 
        Tag = r.Tag }

let areStandardAttrsVisiblyDifferent( a1 : ExchangeAppointmentDTO, a2 : ExchangeAppointmentDTO ) : bool =
    let a1n = normalize( a1 )
    let a2n = normalize( a2 )
    let result = not (( optionstringsAreEqual a1n.CategoriesJSON a2n.CategoriesJSON ) && 
                    ( optionstringsAreEqual a1n.Location a2n.Location ) && ( optionstringsAreEqual a1n.Body a2n.Body ) && ( optionstringsAreEqual a1n.Subject a2n.Subject )
                    && ( a1n.Start = a2n.Start ) && ( a1n.End = a2n.End ) && ( a1n.ReminderMinutesBeforeStart = a2n.ReminderMinutesBeforeStart ) && 
                    ( a1n.IsReminderSet = a2n.IsReminderSet )
                    && ( a1n.Sensitivity = a2n.Sensitivity ))
    if result then
        devlog.Debug( sprintf "StandardAttrsAREVisiblyDifferent for '%A' '%A'" a1n.Id a2n.Id )
        standardAttrsVisiblyDifferentLogger.Debug( sprintf "StandardAttrsAREVisiblyDifferent for '%A' '%A'" a1n a2n )
    if not (result) && exchangeForceTreatAsDiff then
        devlog.Debug( sprintf "StandardAttr same for '%A' '%A', but forced treat as different" a1n.Id a2n.Id )
        standardAttrsVisiblyDifferentLogger.Debug( sprintf "StandardAttr same for '%A' '%A', but forced treat as different" a1n a2n )
        true
    else
        result

let saveExchangeAppointment( app : ExchangeAppointmentDTO, upload : bool, downloadRound : int ) = 
    let db = db()
    let possibleApp = 
        if upload then 
            query {
                for r in db.ExchangeAppointments do
                where ( r.InternalId = app.InternalId )
                select r
            } |> Seq.tryHead
        else 
            query {
                for r in db.ExchangeAppointments do
                where ( r.ExternalId = app.ExternalId )
                select r
            } |> Seq.tryHead

    //logger.Debug( sprintf "upload:'%A', app.InternalId:'%A', app.ExternalId:'%A', possibleApp:'%A' serviceAccountId: '%A' app.LastModifiedTime:'%A'" upload app.InternalId app.ExternalId possibleApp app.ServiceAccountId app.LastModifiedTime )
    devlog.Debug( sprintf "saveExchangeAppointment app.InternalId:'%A', app.ExternalId:'%A'" app.InternalId app.ExternalId )
    if ( possibleApp.IsNone ) then
        let newApp = new SqlConnection.ServiceTypes.ExchangeAppointments()
        copyToExchangeAppointment(newApp, app)
        newApp.Upload <- upload
        newApp.IsNew <- true
        newApp.DownloadRound <- downloadRound
        db.ExchangeAppointments.InsertOnSubmit newApp
    else
        if ( possibleApp.Value.DownloadRound <> downloadRound) then // ignore duplicities received from EWS
            if areStandardAttrsVisiblyDifferent(app, convert(possibleApp.Value)) then
                let originalInternalId = possibleApp.Value.InternalId
                copyToExchangeAppointment(possibleApp.Value, app)
                possibleApp.Value.InternalId <- originalInternalId
                possibleApp.Value.Upload <- upload
                possibleApp.Value.WasJustUpdated <- true
                possibleApp.Value.DownloadRound <- downloadRound
                devlog.Debug ( sprintf "saved:'%A'" possibleApp.Value.Id )
            else
                ignlog.Debug ( sprintf "ignoring:'%A', have same values as '%A'" app.InternalId possibleApp.Value.InternalId )
        else
            ignlog.Debug ( sprintf "ignoring:'%A', have same downloadRound '%A'" app.InternalId downloadRound )

    db.DataContext.SubmitChanges()
        
let ExchangeAppointmentsToUpload( serviceAccountId : int ) = 
    let db = db()
    query {
        for r in db.ExchangeAppointments do
        where ( r.ServiceAccountId = serviceAccountId && r.Upload )
        select (convert(r))
    } |> Seq.toList

let changeExchangeAppointmentExternalId(app : ExchangeAppointmentDTO, externalId : string) =
    let cnn = cnn()
    cnn.ExecuteCommand("UPDATE ExchangeAppointments SET ExternalId = {0} WHERE InternalId = {1}", externalId, app.InternalId ) |> ignore

let setExchangeAppointmentAsUploaded(app : ExchangeAppointmentDTO) =
    let cnn = cnn()
    cnn.ExecuteCommand("UPDATE ExchangeAppointments SET Upload = 0 WHERE InternalId = {0}", app.InternalId ) |> ignore

let prepareForDownload( serviceAccountId : int ) =
    let cnn = cnn()
    cnn.ExecuteCommand("UPDATE ExchangeAppointments SET IsNew=0, WasJustUpdated=0 WHERE ServiceAccountId = {0}", serviceAccountId ) |> ignore

let prepareForUpload() =
    let cnn = cnn()
    cnn.ExecuteCommand("UPDATE ExchangeAppointments SET Upload=1 WHERE Upload=0 and (ExternalID IS NULL OR LEN(ExternalID)=0)" ) |> ignore

let getUpdatedExchangeAppointments() =
    let db = db()
    query {
        for r in db.ExchangeAppointments do
        where ( r.WasJustUpdated )
        select (convert(r))
    } |> Seq.toList

let getNewExchangeAppointments() =
    let db = db()
    query {
        for r in db.ExchangeAppointments do
        where ( r.IsNew )
        select (convert(r))
    } |> Seq.toList
    
let changeInternalIdBecauseOfDuplicitySimple( internalId : Guid, exchangeAppointmentId : int ) =
    let cnn = cnn()
    cnn.ExecuteCommand("UPDATE ExchangeAppointments SET InternalId = {0} WHERE Id = {1}", internalId, exchangeAppointmentId ) |> ignore

let changeInternalIdBecauseOfDuplicity( exchangeAppointment : ExchangeAppointmentDTO, foundDuplicity : AdapterAppointmentDTO ) =
    let cnn = cnn()
    changeInternalIdBecauseOfDuplicitySimple( foundDuplicity.InternalId, exchangeAppointment.Id )

