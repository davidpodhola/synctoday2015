/*
declare @idVal int = 3460
declare @internalIdVal uniqueidentifier = '00000000-0000-0000-0000-000000000000'
declare @externalIdVal nvarchar(max) =  null --'AAMkAGJmMDFhMmE3LTU3YWEtNDNmNy1iZDllLWIxY2QwNTExNzMzNgBGAAAAAAD4AJFMmexLRJIMfiUwKEp/BwDirOIt3rDoR7XCk9dPTeNJAAAAEStQAABZxtoO29MgRaxeM8wqVZAFAAGtrcGdAAA='
*/

declare @id int = @idVal
declare @internalId uniqueidentifier = @internalIdVal
declare @externalId nvarchar(max) = @externalIdVal

SELECT [Id]
      ,[InternalId]
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
      ,[LastUPError]
  FROM [ExchangeAppointments]
  WHERE 
  Id = ( CASE WHEN @Id = 0 THEN Id ELSE @Id End )
  AND InternalId = ( CASE WHEN @InternalId = '00000000-0000-0000-0000-000000000000' THEN InternalId ELSE @InternalId End )
  AND ExternalId = ISNULL(@ExternalId, ExternalId)

