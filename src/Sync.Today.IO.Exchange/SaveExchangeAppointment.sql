-- uncomment for testing

DECLARE @id int
select @id  = @idVal
DECLARE @InternalId uniqueidentifier
select @InternalId  = @InternalIdVal
DECLARE @ExternalId nvarchar(max)
select @ExternalId  = @ExternalIdVal
DECLARE @Body nvarchar(max)
select @Body  = @BodyVal
DECLARE @Start datetime
select @Start  = @StartVal
DECLARE @End datetime
select @End  = @EndVal
DECLARE @LastModified datetime
select @LastModified  = @LastModifiedVal
DECLARE @Location nvarchar(max)
select @Location  = @LocationVal
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
declare @DownloadRound int = @DownloadRoundVal
DECLARE @Subject nvarchar(max)
select @Subject  = @SubjectVal

BEGIN TRAN

UPDATE ExchangeAppointments
   SET --[InternalId] = <InternalId, uniqueidentifier,> NEVER!
      [ExternalId] = @ExternalId
      ,[Body] = @Body
      ,[Start] = @Start
      ,[End] = @End
      ,[LastModifiedTime] = @LastModified
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
      ,[Tag] = ( case when @Tag = -1 then null else @Tag end )
      ,[IsNew] = 0
      ,[WasJustUpdated] = 
		( CASE WHEN 
			ISNULL(ExternalId, '') <> ISNULL(@ExternalId, '')
		  OR ISNULL(Body, '') <> ISNULL(@Body, '')
		  OR ISNULL(Start, '2015-01-01') <> ISNULL(@Start, '2015-01-01')
		  OR ISNULL([End], '2015-01-01') <> ISNULL(@End, '2015-01-01')
		  OR ISNULL(LastModifiedTime, '2015-01-01') <> ISNULL(@LastModified, '2015-01-01')
		  OR ISNULL(Location, '') <> ISNULL(@Location, '')
		  OR ISNULL(IsReminderSet, 0 ) <> ISNULL(@IsReminderSet, 0 )
		  OR ISNULL(AppointmentState, 0 ) <> ISNULL(@AppointmentState, 0 )
		  OR ISNULL([Subject], '') <> ISNULL(@Subject, '')
		  OR ISNULL([RequiredAttendeesJSON], '') <> ISNULL(@RequiredAttendeesJSON, '')
		  OR ISNULL(CategoriesJSON, '') <> ISNULL(@CategoriesJSON, '')
		  OR ISNULL(ServiceAccountId, 0) <> ISNULL(@ServiceAccountId, 0)
		  OR ISNULL(Tag, 0) <> ISNULL(@Tag,0)
		THEN 1 ELSE 0 END )
      ,[DownloadRound] = @DownloadRound
      ,[LastDLError] = @LastDLError
      ,[LastUPError] = @LastUPError
 WHERE 	Id = ISNULL(@id, -1) OR InternalId = ISNULL(@InternalId, 'ABDEB065-DFE0-4E4F-B504-F62841690930') OR ExternalId = ISNULL(@ExternalId, '{ABDEB065-DFE0-4E4F-B504-F62841690930}')

IF @@ROWCOUNT = 0
BEGIN

INSERT INTO [ExchangeAppointments]
           ([InternalId]
           ,[ExternalId]
           ,[Body]
           ,[Start]
           ,[End]
           ,[LastModifiedTime]
           ,[Location]
           ,[IsReminderSet]
           ,[AppointmentState]
           ,[Subject]
           ,[RequiredAttendeesJSON]
           ,[ReminderMinutesBeforeStart]
           ,[Sensitivity]
           ,[RecurrenceJSON]
           ,[ModifiedOccurrencesJSON]
           ,[LastOccurrenceJSON]
           ,[IsRecurring]
           ,[IsCancelled]
           ,[ICalRecurrenceId]
           ,[FirstOccurrenceJSON]
           ,[DeletedOccurrencesJSON]
           ,[AppointmentType]
           ,[Duration]
           ,[StartTimeZone]
           ,[EndTimeZone]
           ,[AllowNewTimeProposal]
           ,[CategoriesJSON]
           ,[ServiceAccountId]
           ,[Upload]
           ,[Tag]
           ,[IsNew]
           ,[WasJustUpdated]
           ,[DownloadRound]
           ,[LastDLError]
           ,[LastUPError])

   SELECT @InternalId
           ,@ExternalId
           ,@Body
           ,@Start
           ,@End
           ,@LastModified
           ,@Location
           ,@IsReminderSet
           ,@AppointmentState
           ,@Subject
           ,@RequiredAttendeesJSON
           ,@ReminderMinutesBeforeStart
           ,@Sensitivity
           ,@RecurrenceJSON
           ,@ModifiedOccurrencesJSON
           ,@LastOccurrenceJSON
           ,@IsRecurring
           ,@IsCancelled
           ,@ICalRecurrenceId
           ,@FirstOccurrenceJSON
           ,@DeletedOccurrencesJSON
           ,@AppointmentType
           ,@Duration
           ,@StartTimeZone
           ,@EndTimeZone
           ,@AllowNewTimeProposal
           ,@CategoriesJSON
           ,@ServiceAccountId
           ,@Upload
           ,( case when @Tag = -1 then null else @Tag end )
           ,1
           ,0
           ,@DownloadRound
           ,@LastDLError
           ,@LastUPError
;
  SELECT @id = SCOPE_IDENTITY()
END

COMMIT

SELECT * FROM ExchangeAppointments
	WHERE Id = ISNULL(@id, -1) OR InternalId = ISNULL(@InternalId, 'ABDEB065-DFE0-4E4F-B504-F62841690930') OR ExternalId = ISNULL(@ExternalId, '{ABDEB065-DFE0-4E4F-B504-F62841690930}')
 