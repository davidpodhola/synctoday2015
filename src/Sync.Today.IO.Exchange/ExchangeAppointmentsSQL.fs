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
    ( new GetExchangeAppointmentsQuery() ).AsyncExecute(0,internalId, null, 0, -1, -1, -1 ) |> Async.RunSynchronously |> Seq.tryHead |> convertOption

let exchangeAppointments () = 
    ( new GetExchangeAppointmentsQuery() ).AsyncExecute(0,Guid.Empty, null, 0, -1, -1, -1 ) |> Async.RunSynchronously |> Seq.map convert


let saveDLUPIssues( externalId : string, lastDLError : string, lastUPError : string  ) = 
    devlog.Debug( ( sprintf "saveDLUPIssues: externalId:'%A', LastDLError:'%A', LastUPError:'%A'" externalId, lastDLError, lastUPError  ) )
    ( new SaveDLUPIssuesQuery() ).AsyncExecute(lastDLError, lastUPError, externalId ) |> Async.RunSynchronously |> ignore

let categories( r : ExchangeAppointmentDTO ) : string array =
    if optionstringIsEmpty r.CategoriesJSON then [| |] else unjson<string array>( r.CategoriesJSON.Value )
    
let normalize ( r : ExchangeAppointmentDTO ) : ExchangeAppointmentDTO =
    { Id = r.Id; InternalId = r.InternalId; ExternalId = r.ExternalId; Body = r.Body; Start = fixDateSecs(r.Start); End = fixDateSecs(r.End); LastModifiedTime = fixDateSecs(r.LastModifiedTime); 
        Location = r.Location;
        IsReminderSet = r.IsReminderSet; AppointmentState = r.AppointmentState; Subject = r.Subject; RequiredAttendeesJSON = r.RequiredAttendeesJSON;
        ReminderMinutesBeforeStart = r.ReminderMinutesBeforeStart; Sensitivity = r.Sensitivity; RecurrenceJSON = r.RecurrenceJSON; ModifiedOccurrencesJSON = r.ModifiedOccurrencesJSON;
        LastOccurrenceJSON = r.LastOccurrenceJSON; IsRecurring = r.IsRecurring; IsCancelled = r.IsCancelled; ICalRecurrenceId = r.ICalRecurrenceId; 
        FirstOccurrenceJSON = r.FirstOccurrenceJSON; 
        DeletedOccurrencesJSON = r.DeletedOccurrencesJSON; AppointmentType = r.AppointmentType; Duration = r.Duration; StartTimeZone = r.StartTimeZone; 
        EndTimeZone = r.EndTimeZone; AllowNewTimeProposal = r.AllowNewTimeProposal; 
        CategoriesJSON = string2optionString (json(Array.FindAll( categories(r), ( fun p -> not(String.IsNullOrWhiteSpace( p ) ) ) ) ) );
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
    ( new SaveExchangeAppointmentsQuery() ).AsyncExecute(
        0,
        app.InternalId,
        optionString2String app.ExternalId,
        optionString2String app.Body,
        app.Start,
        app.End,
        app.LastModifiedTime,
        optionString2String app.Location,
        optionString2String app.CategoriesJSON,
        app.ServiceAccountId,
        upload,
        ( if app.Tag.IsNone then -1 else app.Tag.Value ),
        null,
        null,
        app.IsReminderSet,
        app.AppointmentState,
        optionString2String app.RequiredAttendeesJSON,
        app.ReminderMinutesBeforeStart,
        app.Sensitivity,
        optionString2String app.RecurrenceJSON,
        optionString2String app.ModifiedOccurrencesJSON,
        optionString2String app.LastOccurrenceJSON,
        app.IsRecurring,
        app.IsCancelled,
        optionString2String app.ICalRecurrenceId,
        optionString2String app.FirstOccurrenceJSON,
        optionString2String app.DeletedOccurrencesJSON,
        app.AppointmentType,
        app.Duration,
        optionString2String app.StartTimeZone,
        optionString2String app.EndTimeZone,
        app.AllowNewTimeProposal,
        downloadRound,
        optionString2String app.Subject
    ) |> Async.RunSynchronously
        
let ExchangeAppointmentsToUpload( serviceAccountId : int ) = 
    ( new GetExchangeAppointmentsQuery() ).AsyncExecute(0,Guid.Empty, null, serviceAccountId, -1, -1, -1 ) |> Async.RunSynchronously |> Seq.map convert

type ChangeExternalIdQuery = SqlCommandProvider<"ChangeExternalId.sql", ConnectionStringName>

let changeExchangeAppointmentExternalId(app : ExchangeAppointmentDTO, externalId : string) =
    ( new ChangeExternalIdQuery() ).AsyncExecute(app.InternalId, externalId) |> Async.RunSynchronously |> ignore

type SetExchangeAppointmentAsUploadedQuery = SqlCommandProvider<"SetExchangeAppointmentAsUploaded.sql", ConnectionStringName>

let setExchangeAppointmentAsUploaded(app : ExchangeAppointmentDTO) =
    ( new SetExchangeAppointmentAsUploadedQuery() ).AsyncExecute(app.InternalId) |> Async.RunSynchronously |> ignore

type PrepareForDownloadQuery = SqlCommandProvider<"PrepareForDownload.sql", ConnectionStringName>

let prepareForDownload( serviceAccountId : int ) =
    ( new PrepareForDownloadQuery() ).AsyncExecute(serviceAccountId) |> Async.RunSynchronously |> ignore

type PrepareForUploadQuery = SqlCommandProvider<"PrepareForUpload.sql", ConnectionStringName>

let prepareForUpload() =
    ( new PrepareForUploadQuery() ).AsyncExecute() |> Async.RunSynchronously |> ignore

let getUpdatedExchangeAppointments() =
    ( new GetExchangeAppointmentsQuery() ).AsyncExecute(0,Guid.Empty, null, 0, -1, 1, -1 ) |> Async.RunSynchronously |> Seq.map convert

let getNewExchangeAppointments() =
    ( new GetExchangeAppointmentsQuery() ).AsyncExecute(0,Guid.Empty, null, 0, -1, -1, 1 ) |> Async.RunSynchronously |> Seq.map convert
    
type ChangeInternalIdQuery = SqlCommandProvider<"ChangeInternalId.sql", ConnectionStringName>

let changeInternalIdBecauseOfDuplicitySimple( internalId : Guid, exchangeAppointmentId : int ) =
    ( new ChangeInternalIdQuery() ).AsyncExecute(internalId, exchangeAppointmentId )  |> Async.RunSynchronously |> ignore

let changeInternalIdBecauseOfDuplicity( exchangeAppointment : ExchangeAppointmentDTO, foundDuplicityAdapterAppointmentDTOInternalId ) =
    changeInternalIdBecauseOfDuplicitySimple( foundDuplicityAdapterAppointmentDTOInternalId, exchangeAppointment.Id )

type GetOldAppointmentsQuery = SqlCommandProvider<"GetOldAppointments.sql", ConnectionStringName>

let internal convert2( r : GetOldAppointmentsQuery.Record ) : ExchangeAppointmentDTO =
    { Id = r.Id; InternalId = r.InternalId; ExternalId = r.ExternalId; Body = r.Body; Start = r.Start; End = r.End; LastModifiedTime = r.LastModifiedTime; Location = r.Location;
                    IsReminderSet = r.IsReminderSet; AppointmentState = r.AppointmentState; Subject = r.Subject; RequiredAttendeesJSON = r.RequiredAttendeesJSON;
                    ReminderMinutesBeforeStart = r.ReminderMinutesBeforeStart; Sensitivity = r.Sensitivity; RecurrenceJSON = r.RecurrenceJSON; ModifiedOccurrencesJSON = r.ModifiedOccurrencesJSON;
                    LastOccurrenceJSON = r.LastOccurrenceJSON; IsRecurring = r.IsRecurring; IsCancelled = r.IsCancelled; ICalRecurrenceId = r.ICalRecurrenceId; 
                    FirstOccurrenceJSON = r.FirstOccurrenceJSON; 
                    DeletedOccurrencesJSON = r.DeletedOccurrencesJSON; AppointmentType = r.AppointmentType; Duration = r.Duration; StartTimeZone = r.StartTimeZone; 
                    EndTimeZone = r.EndTimeZone; AllowNewTimeProposal = r.AllowNewTimeProposal; CategoriesJSON = r.CategoriesJSON; ServiceAccountId = r.ServiceAccountId; 
                    Tag = r.Tag }


let getOldAppointments serviceAccountId =
    ( new GetOldAppointmentsQuery() ).AsyncExecute( serviceAccountId ) |> Async.RunSynchronously |> Seq.map ( fun t -> convert2(t) )

