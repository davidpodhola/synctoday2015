-- uncomment for testing
/*  
DECLARE @idVal int = 0
DECLARE @InternalIdVal uniqueidentifier = '40995F91-6236-4E5F-B289-0967E18D1E67'
DECLARE @ExternalIdVal nvarchar(2048) = NULL
DECLARE @DescriptionVal nvarchar(max) = 'Our event description'
DECLARE @StartVal datetime = '2015-05-05 10:35:42.213'
DECLARE @EndVal datetime = '2015-05-05 10:36:42.213'
DECLARE @LastModifiedVal datetime = '2015-05-05 10:36:42.213'
DECLARE @LocationVal nvarchar(max) = 'Here'
DECLARE @SummaryVal nvarchar(max) = 'Title635664189422126569'
DECLARE @CategoriesJSONVal nvarchar(max) = NULL
DECLARE @ServiceAccountIdVal int = 3
DECLARE @UploadVal bit = 1
DECLARE @TagVal int = 0
DECLARE @LastDLErrorVal nvarchar(max) = NULL
DECLARE @LastUPErrorVal nvarchar(max) = NULL
*/

DECLARE @id int
select @id  = @idVal
DECLARE @InternalId uniqueidentifier
select @InternalId  = @InternalIdVal
DECLARE @ExternalId nvarchar(max)
select @ExternalId  = @ExternalIdVal
DECLARE @Description nvarchar(max)
select @Description  = @DescriptionVal
DECLARE @Start datetime
select @Start  = @StartVal
DECLARE @End datetime
select @End  = @EndVal
DECLARE @LastModified datetime
select @LastModified  = @LastModifiedVal
DECLARE @Location nvarchar(max)
select @Location  = @LocationVal
DECLARE @Summary nvarchar(max)
select @Summary  = @SummaryVal
DECLARE @CategoriesJSON nvarchar(max)
select @CategoriesJSON  = @CategoriesJSONVal
DECLARE @ServiceAccountId int
select @ServiceAccountId  = @ServiceAccountIdVal
DECLARE @Upload bit
select @Upload  = @UploadVal
DECLARE @Tag int
select @Tag  = @TagVal
DECLARE @LastDLError nvarchar(max)
select @LastDLError  = @LastDLErrorVal
DECLARE @LastUPError nvarchar(max)
select @LastUPError  = @LastUPErrorVal
DECLARE @IsReminderSet bit
select @IsReminderSet  = @IsReminderSetVal
DECLARE @AppointmentState tinyint

BEGIN TRAN

USE [SyncToday2015.new]
GO

UPDATE [dbo].[ExchangeAppointments]
   SET --[InternalId] = <InternalId, uniqueidentifier,> NEVER!
      ,[ExternalId] = @ExternalId
      ,[Body] = @Body
      ,[Start] = @Start
      ,[End] = @End
      ,[LastModifiedTime] = @LastModifiedTime
      ,[Location] = @Location
      ,[IsReminderSet] = @IsReminderSet
      ,[AppointmentState] = @AppointmentState, tinyint,>
      ,[Subject] = @Subject, nvarchar(max),>
      ,[RequiredAttendeesJSON] = @RequiredAttendeesJSON, nvarchar(max),>
      ,[ReminderMinutesBeforeStart] = @ReminderMinutesBeforeStart, int,>
      ,[Sensitivity] = @Sensitivity, tinyint,>
      ,[RecurrenceJSON] = @RecurrenceJSON, nvarchar(max),>
      ,[ModifiedOccurrencesJSON] = @ModifiedOccurrencesJSON, nvarchar(max),>
      ,[LastOccurrenceJSON] = @LastOccurrenceJSON, nvarchar(max),>
      ,[IsRecurring] = @IsRecurring, bit,>
      ,[IsCancelled] = @IsCancelled, bit,>
      ,[ICalRecurrenceId] = @ICalRecurrenceId, nvarchar(max),>
      ,[FirstOccurrenceJSON] = @FirstOccurrenceJSON, nvarchar(max),>
      ,[DeletedOccurrencesJSON] = @DeletedOccurrencesJSON, nvarchar(max),>
      ,[AppointmentType] = @AppointmentType, tinyint,>
      ,[Duration] = @Duration, int,>
      ,[StartTimeZone] = @StartTimeZone, nvarchar(max),>
      ,[EndTimeZone] = @EndTimeZone, nvarchar(max),>
      ,[AllowNewTimeProposal] = @AllowNewTimeProposal, bit,>
      ,[CategoriesJSON] = @CategoriesJSON, nvarchar(max),>
      ,[ServiceAccountId] = @ServiceAccountId, int,>
      ,[Upload] = @Upload, bit,>
      ,[Tag] = @Tag, int,>
      ,[IsNew] = @IsNew, bit,>
      ,[WasJustUpdated] = @WasJustUpdated, bit,>
      ,[DownloadRound] = @DownloadRound, int,>
      ,[LastDLError] = @LastDLError, nvarchar(max),>
      ,[LastUPError] = @LastUPError, nvarchar(max),>
 WHERE <Search Conditions,,>
GO


UPDATE CalDavEvents with (serializable) SET 
	   --InternalId = @InternalId NO, we have ChangeInternalIdBecauseOfDuplicity for this
      ExternalId = @ExternalId
      ,[Description] = @Description
      ,Start = @Start
      ,[End] = @End
      ,LastModified = @LastModified
      ,Location = @Location
      ,Summary = @Summary
      ,CategoriesJSON = @CategoriesJSON
      ,ServiceAccountId = @ServiceAccountId
      ,Upload = @Upload
      ,Tag = @Tag
      ,IsNew = 0
      ,WasJustUpdated = 
		( CASE WHEN 
			ISNULL(ExternalId, '') <> ISNULL(@ExternalId, '')
		  OR ISNULL([Description], '') <> ISNULL(@Description, '')
		  OR ISNULL(Start, '2015-01-01') <> ISNULL(@Start, '2015-01-01')
		  OR ISNULL([End], '2015-01-01') <> ISNULL(@End, '2015-01-01')
		  OR ISNULL(LastModified, '2015-01-01') <> ISNULL(@LastModified, '2015-01-01')
		  OR ISNULL(Location, '') <> ISNULL(@Location, '')
		  OR ISNULL(Summary, '') <> ISNULL(@Summary, '')
		  OR ISNULL(CategoriesJSON, '') <> ISNULL(@CategoriesJSON, '')
		  OR ISNULL(ServiceAccountId, 0) <> ISNULL(@ServiceAccountId, 0)
		  OR ISNULL(Tag, 0) <> ISNULL(@Tag,0)
		THEN 1 ELSE 0 END )
      ,LastDLError = @LastDLError
      ,LastUPError = @LastUPError
	WHERE Id = ISNULL(@id, -1) OR InternalId = ISNULL(@InternalId, 'ABDEB065-DFE0-4E4F-B504-F62841690930') OR ExternalId = ISNULL(@ExternalId, '{ABDEB065-DFE0-4E4F-B504-F62841690930}')

IF @@ROWCOUNT = 0
BEGIN
  INSERT CalDavEvents(
	   InternalId
      ,ExternalId
      ,[Description]
      ,Start
      ,[End]
      ,LastModified
      ,Location
      ,Summary
      ,CategoriesJSON
      ,ServiceAccountId
      ,Upload
      ,Tag
      ,IsNew
      ,WasJustUpdated
      ,LastDLError
      ,LastUPError
		   ) 
   SELECT @InternalId, @ExternalId, @Description, @Start, @End, @LastModified, @Location, @Summary, @CategoriesJSON, @ServiceAccountId,
			@Upload, @Tag, 1, 0, @LastDLError, @LastUPError
;
  SELECT @id = SCOPE_IDENTITY()
END

COMMIT

SELECT * FROM CalDavEvents
	WHERE Id = ISNULL(@id, -1) OR InternalId = ISNULL(@InternalId, 'ABDEB065-DFE0-4E4F-B504-F62841690930') OR ExternalId = ISNULL(@ExternalId, '{ABDEB065-DFE0-4E4F-B504-F62841690930}')
 