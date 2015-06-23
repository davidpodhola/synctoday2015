-- uncomment for testing

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
select @AppointmentState  = @AppointmentStateVal
declare @RequiredAttendeesJSON nvarchar(max) = @RequiredAttendeesJSONVal
declare @ReminderMinutesBeforeStart int = @ReminderMinutesBeforeStartVal
declare @Sensitivity tinyint = @SensitivityVal
declare @RecurrenceJSON nvarchar(max) = @RecurrenceJSONVal
declare @ModifiedOccurrencesJSON nvarchar(max) = @ModifiedOccurrencesJSONVal
declare @LastOccurrenceJSON nvarchar(max) = @LastOccurrenceJSONVal
declare @IsRecurring bit = @IsRecurringVal
declare @IsCancelled bit = @IsCancelledVal
declare @ICalRecurrenceId nvarchar(max) = @ICalRecurrenceIdVal
declare @FirstOccurrenceJSON nvarchar(max) = @FirstOccurrenceJSONVal
declare @DeletedOccurrencesJSON nvarchar(max) = @DeletedOccurrencesJSONVal
declare @AppointmentType tinyint = @AppointmentTypeVal
declare @Duration int = @DurationVal
declare @StartTimeZone nvarchar(max) = @StartTimeZoneVal
declare @EndTimeZone nvarchar(max) = @EndTimeZoneVal
declare @AllowNewTimeProposal bit = @AllowNewTimeProposalVal
declare @CategoriesJSON nvarchar(max) = @CategoriesJSONVal
declare @DownloadRound int = @DownloadRoundVal
declare @LastDLError nvarchar(max) = @LastDLErrorVal
declare @LastUPError nvarchar(max) = @LastUPErrorVal

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
      ,[AppointmentState] = @AppointmentState
      ,[Subject] = @Subject
      ,[RequiredAttendeesJSON] = @RequiredAttendeesJSON
      ,[ReminderMinutesBeforeStart] = @ReminderMinutesBeforeStart
      ,[Sensitivity] = @Sensitivity
      ,[RecurrenceJSON] = @RecurrenceJSON
      ,[ModifiedOccurrencesJSON] = @ModifiedOccurrencesJSON
      ,[LastOccurrenceJSON] = @LastOccurrenceJSON
      ,[IsRecurring] = @IsRecurring
      ,[IsCancelled] = @IsCancelled
      ,[ICalRecurrenceId] = @ICalRecurrenceId
      ,[FirstOccurrenceJSON] = @FirstOccurrenceJSON
      ,[DeletedOccurrencesJSON] = @DeletedOccurrencesJSON
      ,[AppointmentType] = @AppointmentType
      ,[Duration] = @Duration
      ,[StartTimeZone] = @StartTimeZone
      ,[EndTimeZone] = @EndTimeZone
      ,[AllowNewTimeProposal] = @AllowNewTimeProposal
      ,[CategoriesJSON] = @CategoriesJSON
      ,[ServiceAccountId] = @ServiceAccountId
      ,[Upload] = @Upload
      ,[Tag] = @Tag
      ,[IsNew] = @IsNew
      ,[WasJustUpdated] = @WasJustUpdated
      ,[DownloadRound] = @DownloadRound
      ,[LastDLError] = @LastDLError
      ,[LastUPError] = @LastUPError
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
 