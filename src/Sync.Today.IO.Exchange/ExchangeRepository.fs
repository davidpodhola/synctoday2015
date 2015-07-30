module ExchangeRepository

open System
open Microsoft.Exchange.WebServices.Data
open System.Configuration
open sync.today.Models
open Common
open ExchangeAppointmentsSQL
open FSharp.Data
open sync.today.cipher
open Schemas
open ExchangeCommon

let logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

let public EXCHANGE_SERVICE_KEY="EXCHANGE"


let propertySet = 
    if exchangeVersion <> ExchangeVersion.Exchange2007_SP1 then
        let result = PropertySet( Array.append Properties [| AppointmentSchemaStartTimeZone; AppointmentSchemaEndTimeZone  |] )
        result.RequestedBodyType <- Nullable(BodyType.Text)
        result
    else
        let result = PropertySet( Properties )
        result.RequestedBodyType <- Nullable(BodyType.Text)
        result

let shouldBeReminderSet =
    let posVal = ConfigurationManager.AppSettings.[ "Exchange.shouldBeReminderSet" ]
    let result = ref false
    if Boolean.TryParse( posVal, result ) then
        result.Value
    else
        true

let copyDTOToAppointment( r : Appointment, source : ExchangeAppointmentDTO )  =
        r.Body <- MessageBody(BodyType.Text, ( if optionstringIsEmpty source.Body then String.Empty else source.Body.Value ) )

        if exchangeUseTimeZone && (exchangeVersion <> ExchangeVersion.Exchange2007_SP1) then
            r.StartTimeZone <- timezone(false)
        r.Start <- source.Start
        r.End <- source.End 

        if exchangeUseTimeZone && (exchangeVersion <> ExchangeVersion.Exchange2007_SP1) then
            r.EndTimeZone <- timezone(false)

        r.Location <- optionString2String source.Location 
        r.ReminderMinutesBeforeStart <- source.ReminderMinutesBeforeStart
        r.Subject <- optionString2String source.Subject 
        if shouldBeReminderSet then
            r.IsReminderSet <- source.IsReminderSet 
        else 
            r.IsReminderSet <- false

        // Categories
        let oldCategories = json(r.Categories)
        devlog.Debug( sprintf "InternalId: %A Subject: %A oldCategories:'%A' source.CategoriesJSON:''%A " source.InternalId r.Subject oldCategories source.CategoriesJSON )
        if oldCategories <> optionString2String source.CategoriesJSON then 
            let categories = if optionstringIsEmpty source.CategoriesJSON then [| |] else unjson<string array>( source.CategoriesJSON.Value )
            let categoriesNotEmpty = Array.FindAll(categories, ( fun p -> not(String.IsNullOrWhiteSpace(p) ) ) )
            r.Categories.Clear()
            devlog.Debug( sprintf "categoriesNotEmpty:'%A'" categoriesNotEmpty )
            r.Categories.AddRange( categoriesNotEmpty )

let copyAppointmentToDTO( r : Appointment, serviceAccountId : int, tag : int option ) : ExchangeAppointmentDTO =
    try
        { Id = 0; InternalId = Guid.NewGuid(); ExternalId = Some(r.Id.ToString());     
        Body = string2optionString r.Body.Text; Start = r.Start; End = r.End; LastModifiedTime = r.LastModifiedTime; Location = string2optionString r.Location;
        IsReminderSet = r.IsReminderSet; AppointmentState = byte r.AppointmentState; Subject = string2optionString r.Subject; 
        RequiredAttendeesJSON = Some(json(r.RequiredAttendees));
        ReminderMinutesBeforeStart = r.ReminderMinutesBeforeStart; Sensitivity = byte r.Sensitivity; 
        RecurrenceJSON = ( if exchangeVersion <> ExchangeVersion.Exchange2007_SP1 then Some(json(r.Recurrence)) else None ); 
        ModifiedOccurrencesJSON = ( if exchangeVersion <> ExchangeVersion.Exchange2007_SP1 then Some(json(r.ModifiedOccurrences)) else None );
        LastOccurrenceJSON = ( if exchangeVersion <> ExchangeVersion.Exchange2007_SP1  then Some(json(r.LastOccurrence)) else None ); 
        IsRecurring = ( if exchangeVersion <> ExchangeVersion.Exchange2007_SP1 then r.IsRecurring else false); IsCancelled = r.IsCancelled; 
        ICalRecurrenceId = None; 
        FirstOccurrenceJSON = ( if exchangeVersion <> ExchangeVersion.Exchange2007_SP1  then Some(json(r.FirstOccurrence)) else None ); 
        DeletedOccurrencesJSON = ( if exchangeVersion <> ExchangeVersion.Exchange2007_SP1  then Some(json(r.DeletedOccurrences)) else None ); 
        AppointmentType = byte r.AppointmentType; Duration = int r.Duration.TotalMinutes; 
        StartTimeZone = ( if exchangeVersion <> ExchangeVersion.Exchange2007_SP1  then Some(r.StartTimeZone.StandardName) else None ); 
        EndTimeZone = ( if exchangeVersion <> ExchangeVersion.Exchange2007_SP1  then Some(r.EndTimeZone.StandardName) else None );  
        AllowNewTimeProposal = false; CategoriesJSON = Some(json(r.Categories)); 
        ServiceAccountId = serviceAccountId; 
        Tag = tag }
    with
        | ex -> raise (System.ArgumentException("copyAppointmentToDTO failed", ex)) 

let save( app : Appointment, serviceAccountId : int, downloadRound : int ) =
    saveExchangeAppointment(copyAppointmentToDTO(app, serviceAccountId, None), false, downloadRound)

let insertOrUpdate( app : ExchangeAppointmentDTO ) =
    let downloadRound = int DateTime.Now.Ticks
    saveExchangeAppointment(app, true, downloadRound)


let changeExternalId( app : ExchangeAppointmentDTO, externalId : string ) =
    changeExchangeAppointmentExternalId(app, externalId)

let findFolderByName( _service : ExchangeService, name, login : Login ) : Folder option = 
    ExchangeCommon.findFolderByName( _service, name, login, WellKnownFolderName.Calendar )

let get login externalId =
    devlog.Debug( sprintf "get started %A %A" login externalId )
    let _service = connect(login)

    let folder = 
        if not (login.impersonate) && not( String.IsNullOrWhiteSpace( login.email ) ) && login.email <> login.userName then
            Folder.Bind(_service, new FolderId(WellKnownFolderName.Calendar, new Mailbox(login.email)))
        else
            Folder.Bind(_service, WellKnownFolderName.Calendar)

    try 
        let possibleApp = Appointment.Bind(_service, new ItemId(externalId))        
        devlog.Debug( sprintf "possibleApp %A" possibleApp )
        devlog.Debug( sprintf "possibleApp %A x %A (%A)" possibleApp.ParentFolderId.UniqueId folder.Id.UniqueId possibleApp.ParentFolderId.UniqueId <> folder.Id.UniqueId )
        // check if the parent is my folder
        if possibleApp.ParentFolderId.UniqueId <> folder.Id.UniqueId then
            // if not, nothing
            None
        else
            Some( possibleApp )
    with         
        | ex -> 
            devlog.Debug( ( sprintf "failed for %A %A" login externalId ), ex )
            None               

let download fromDate login =
    let date : DateTime = 
        if login.maintenance && fromDate > DateTime.Now.AddDays(float -30) then DateTime.Now.AddDays(float -30) else fromDate
    logger.Debug( sprintf "download started for %A from %A, maintenance %A" login.userName date login.maintenance )
    prepareForDownload login.serviceAccountId login.maintenance
    let greaterthanfilter = new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.LastModifiedTime, date)
    let filter = new SearchFilter.SearchFilterCollection(LogicalOperator.And, greaterthanfilter)
    let _service = connect(login)

    let syncTodayFolder = findFolderByName( _service, login.folder, login ) 
    if syncTodayFolder.IsSome then 
        let folder = 
            if not (login.impersonate) && not( String.IsNullOrWhiteSpace( login.email ) ) && login.email <> login.userName then
                Folder.Bind(_service, new FolderId(WellKnownFolderName.Calendar, new Mailbox(login.email)))
            else
                Folder.Bind(_service, WellKnownFolderName.Calendar)
        let view = new ItemView(1000)
        view.Offset <- 0
        let mutable search = true
        let downloadRound = int DateTime.Now.Ticks
        while search do
            let found = folder.FindItems(filter, view)
            search <- found.Items.Count = view.PageSize
            view.Offset <- view.Offset + view.PageSize
            logger.DebugFormat( "got {0} items", found.Items.Count )
            for item in found do
                if ( item :? Appointment ) then
                    try
                        let app = item :?> Appointment
                        //logger.Debug( sprintf "processing '%A' " app.Id )
                        app.Load( propertySet )
                        if ( app.LastModifiedTime > date ) then
                            save(app, login.serviceAccountId, downloadRound ) |> ignore
                    with
                        | ex ->
                            saveDLUPIssues(item.Id.ToString(), ex.ToString(), null ) 
                            reraise()                        
    else 
        logger.Warn( sprintf "folder %A not found" login.folder )
                        

    // in maintenance mode we need to iterate through all not just downloaded items
    if login.maintenance then
        getOldAppointments login.serviceAccountId
        |> Seq.filter ( fun p -> not(optionstringIsEmpty p.ExternalId ) )
        |> Seq.filter ( fun p ->
            let potentialItem = get login p.ExternalId.Value
            devlog.Debug( sprintf "potentialItem %A" potentialItem )
            potentialItem.IsNone
        )
        |> Seq.iter ( fun p -> 
            devlog.Debug( sprintf "Marking as deleted %A" p )
            AppointmentRepository.markAppointmentAsDeleted p.InternalId p.ExternalId.Value
        )

    logger.Debug( "download successfully finished" )

let deleteAll(login : Login) =
    let _service = connect(login)
    let folder = Folder.Bind(_service, WellKnownFolderName.Calendar)
    let view = new ItemView(1000)
    view.Offset <- 0
    let mutable search = true
    while search do
        let found = folder.FindItems(view)
        search <- found.Items.Count = view.PageSize
        view.Offset <- view.Offset + view.PageSize
        logger.DebugFormat( "got {0} items", found.Items.Count )
        for item in found do
            if ( item :? Appointment ) then
                let app = item :?> Appointment
                app.Delete( DeleteMode.HardDelete, SendCancellationsMode.SendToNone )

let private createAppointment( item : ExchangeAppointmentDTO, _service : ExchangeService ) : Appointment =
    let app = new Appointment(_service)
    copyDTOToAppointment( app, item )
    app



let upload( login : Login ) =
    logger.Debug( sprintf "upload started, maintenance %A" login.maintenance )
    prepareForUpload login.maintenance
    let _service = connect(login)
    let itemsToUpload = ExchangeAppointmentsToUpload(login.serviceAccountId)

    let folder = 
        if not (login.impersonate) && not( String.IsNullOrWhiteSpace( login.email ) ) && login.email <> login.userName then
            Folder.Bind(_service, new FolderId(WellKnownFolderName.Calendar, new Mailbox(login.email)))
        else
            Folder.Bind(_service, WellKnownFolderName.Calendar)

    for item in itemsToUpload do
        logger.Debug( sprintf "uploading '%A'-'%A'" item.InternalId item.ExternalId )
        if optionstringIsEmpty item.ExternalId then
            let app = createAppointment( item, _service )
            app.Save(folder.Id, SendInvitationsMode.SendToNone)
            logger.Debug( sprintf "'%A' saved" app.Id )
            changeExternalId( item, app.Id.ToString() )
            setExchangeAppointmentAsUploaded(item)
        else
            try 
                let possibleApp = Appointment.Bind(_service, new ItemId(item.ExternalId.Value))
                copyDTOToAppointment( possibleApp, item )
                possibleApp.Update(ConflictResolutionMode.AutoResolve, SendInvitationsOrCancellationsMode.SendToNone)

                // check if the parent is my folder
                if possibleApp.ParentFolderId.UniqueId <> folder.Id.UniqueId then
                  // if not, move
                  devlog.Debug( sprintf "Moving %A from %A to %A" possibleApp possibleApp.ParentFolderId folder )
                  changeExternalId( item, possibleApp.Move( folder.Id ).Id.ToString() )
                  devlog.Debug( "Moved" )

                logger.Debug( sprintf "'%A' saved" possibleApp.Id )
                setExchangeAppointmentAsUploaded(item)
            with 
                | ex -> 
                        saveDLUPIssues(item.ExternalId.Value, null, ex.ToString() ) 
                        try 
                            logger.Debug( sprintf "Save '%A' failed '%A'" item ex )
                            if  ex.Message <> "Set action is invalid for property" then
                                // create new item
                                let app = createAppointment( item, _service )
                                app.Save(folder.Id, SendInvitationsMode.SendToNone)
                                changeExternalId( item, app.Id.ToString() )
                                // we are not able to delete old item, since we were not able to update it
                        with
                            | ex ->
                                saveDLUPIssues(item.ExternalId.Value, null, ex.ToString() ) 
                                reraise()

    if login.maintenance then
        getAppointmentsToBeDeleted login.serviceAccountId
        |> Seq.filter ( fun p -> not(optionstringIsEmpty p.ExternalId ) )
        |> Seq.iter ( fun p -> 
            devlog.Debug( sprintf "deleting %A" p )
            try 
                let possibleApp = Appointment.Bind(_service, new ItemId(p.ExternalId.Value))
                (* 
                possibleApp.Subject <- possibleApp.Subject + " (deleted)"
                possibleApp.LegacyFreeBusyStatus <- LegacyFreeBusyStatus.Free
                possibleApp.Update(ConflictResolutionMode.AutoResolve, SendInvitationsOrCancellationsMode.SendToNone)
                *)
                possibleApp.Delete(DeleteMode.MoveToDeletedItems, SendCancellationsMode.SendToNone)
            with 
            | ex ->
                saveDLUPIssues(p.ExternalId.Value, null, ex.ToString() ) 
            
        )

    logger.Debug( "upload successfully finished" )

let Updated() =
    getUpdatedExchangeAppointments()

let New() =
    getNewExchangeAppointments()

let ConvertToDTO( r : ExchangeAppointmentDTO, adapterId ) : AdapterAppointmentDTO =
   { Id = 0; InternalId = r.InternalId; LastModified = r.LastModifiedTime; Category = findCategoryO( r.CategoriesJSON ); 
   Location = r.Location; Content = r.Body; Title = r.Subject; 
   DateFrom = r.Start; DateTo = r.End; Notification = r.IsReminderSet; IsPrivate = r.Sensitivity <> byte 0; Priority = byte 0; 
   AppointmentId = 0; AdapterId = adapterId; Tag = r.Tag; ReminderMinutesBeforeStart=r.ReminderMinutesBeforeStart }

let getEmpty(old : ExchangeAppointmentDTO option): ExchangeAppointmentDTO =
    if ( old.IsSome ) then
        old.Value
    else 
        { Id = 0; InternalId = Guid.Empty; ExternalId = None; Body = None; Start = DateTime.Now;
            End = DateTime.Now; LastModifiedTime = DateTime.Now; Location = None;
            IsReminderSet = true; 
            AppointmentState = byte 0; Subject = None; RequiredAttendeesJSON = None;
            ReminderMinutesBeforeStart = 15; 
            Sensitivity = byte 0; RecurrenceJSON = None; 
            ModifiedOccurrencesJSON = None;
            LastOccurrenceJSON = None; IsRecurring = false; 
            IsCancelled = false; ICalRecurrenceId = None; 
            FirstOccurrenceJSON = None; 
            DeletedOccurrencesJSON = None; AppointmentType = byte 0; 
            Duration = 0; StartTimeZone = None; 
            EndTimeZone = None; AllowNewTimeProposal = false; 
            CategoriesJSON = None; ServiceAccountId = 0; 
            Tag = None }
        

let ConvertFromDTO( r : AdapterAppointmentDTO, serviceAccountId, original : ExchangeAppointmentDTO ) : ExchangeAppointmentDTO =
    { Id = original.Id; InternalId = r.InternalId; ExternalId = original.ExternalId; Body = r.Content; 
    Start = r.DateFrom; 
    End = r.DateTo; LastModifiedTime = r.LastModified; Location = r.Location;
        IsReminderSet = r.Notification; 
        AppointmentState = original.AppointmentState; Subject = r.Title; 
        RequiredAttendeesJSON = original.RequiredAttendeesJSON;
        ReminderMinutesBeforeStart = r.ReminderMinutesBeforeStart; 
        Sensitivity = original.Sensitivity; RecurrenceJSON = original.RecurrenceJSON; 
        ModifiedOccurrencesJSON = original.ModifiedOccurrencesJSON;
        LastOccurrenceJSON = original.LastOccurrenceJSON; IsRecurring = original.IsRecurring; 
        IsCancelled = original.IsCancelled; ICalRecurrenceId = original.ICalRecurrenceId; 
        FirstOccurrenceJSON = original.FirstOccurrenceJSON; 
        DeletedOccurrencesJSON = original.DeletedOccurrencesJSON; AppointmentType = original.AppointmentType; 
        Duration = int (r.DateTo.Subtract( r.DateTo ).TotalMinutes ); StartTimeZone = original.StartTimeZone; 
        EndTimeZone = original.EndTimeZone; AllowNewTimeProposal = original.AllowNewTimeProposal; 
        CategoriesJSON = AppointmentLevelRepository.replaceCategoryInJSONO( original.CategoriesJSON, r.Category ); 
        ServiceAccountId = serviceAccountId; 
        Tag = r.Tag }

let DownloadForServiceAccount( serviceAccount : ServiceAccountDTO ) =
    let lastSuccessfulDownload = getLastSuccessfulDate2 serviceAccount.LastSuccessfulDownload
    let maintenance = ( DateTime.Now.Date - lastSuccessfulDownload.Date ) > TimeSpan.FromHours( float 1 )
    download lastSuccessfulDownload  ( getLogin serviceAccount.LoginJSON serviceAccount.Id maintenance )

let Download( serviceAccount : ServiceAccountDTO ) =
    ServiceAccountRepository.Download( serviceAccount, DownloadForServiceAccount )

let ChangeInternalIdBecauseOfDuplicity( exchangeAppointment : ExchangeAppointmentDTO, foundDuplicity : AdapterAppointmentDTO ) =
    changeInternalIdBecauseOfDuplicity( exchangeAppointment , foundDuplicity )

let ChangeInternalIdBecauseOfDuplicitySimple( internalId : Guid, exchangeAppointmentId : int ) =
    changeInternalIdBecauseOfDuplicitySimple( internalId , exchangeAppointmentId )

let ExchangeAppointmentInternalIds() =
    exchangeAppointmentInternalIds()

let ExchangeAppointmentByInternalId( internalId : Guid ) =
    exchangeAppointmentByInternalId( internalId )

let Upload( serviceAccount : ServiceAccountDTO ) =
    ServiceAccountRepository.Upload( serviceAccount, ( uploadForServiceAccount upload ) )

let printContent( before : bool ) =
    let data_before = log4net.LogManager.GetLogger("exchange_data_before");
    let data_after = log4net.LogManager.GetLogger("exchange_data_after");
    let logger = if before then data_before else data_after
    logger.Debug("started")
    let exchangeAppointments = exchangeAppointments()
    for exchangeAppointment in exchangeAppointments do
        let replacedBody = if not(optionstringIsEmpty exchangeAppointment.Body) then exchangeAppointment.Body.Value.Replace(System.Environment.NewLine, " ") else String.Empty
        logger.Info( sprintf "%A\t%A\t%A\t%A\t%A\t%A" exchangeAppointment.InternalId exchangeAppointment.Subject exchangeAppointment.Start exchangeAppointment.End exchangeAppointment.LastModifiedTime replacedBody )
    logger.Debug("done")

let AllEvents() =
    exchangeAppointments()

let getExchangeServerAppointments login processEvent =
    logger.Debug( sprintf "getExchangeServerAppointments started for '%A'" login.userName )
    prepareForDownload login.serviceAccountId login.maintenance
    let _service = connect(login)
    let folder = 
        if not (login.impersonate) && not( String.IsNullOrWhiteSpace( login.email ) ) && login.email <> login.userName then
            Folder.Bind(_service, new FolderId(WellKnownFolderName.Calendar, new Mailbox(login.email)))
        else
            Folder.Bind(_service, WellKnownFolderName.Calendar)
    let view = new ItemView(1000)
    view.Offset <- 0
    let downloadRound = int DateTime.Now.Ticks
    seq {
        let search = ref true
        while search.Value do
            let found = folder.FindItems(view)
            search := found.Items.Count = view.PageSize
            view.Offset <- view.Offset + view.PageSize
            logger.DebugFormat( "got {0} items", found.Items.Count )
            for item in found do
                if ( item :? Appointment ) then
                        let app = item :?> Appointment
                        //logger.Debug( sprintf "processing '%A' " app.Id )
                        app.Load( propertySet )
                        yield processEvent app
    }
    

let processExchangeServer login processEvent =
    getExchangeServerAppointments login processEvent