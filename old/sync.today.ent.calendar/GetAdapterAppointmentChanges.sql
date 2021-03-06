--declare @InternalId uniqueidentifier = '72E1D578-E8CD-4729-BF90-026750B1EDDD'

SELECT A.[InternalId]
      ,A.[LastModified]
      ,CASE WHEN O.[Category] = A.Category THEN NULL ELSE A.Category END as Category
      ,CASE WHEN O.[Location] = A.[Location] THEN NULL ELSE A.[Location] END as [Location]
      ,CASE WHEN O.[Content] = A.[Content] THEN NULL ELSE A.[Content] END as [Content]
      ,CASE WHEN O.[Title] = A.[Title] THEN NULL ELSE A.[Title] END as Title
      ,CASE WHEN O.[DateFrom] = A.[DateFrom] THEN NULL ELSE A.[DateFrom] END as [DateFrom]
      ,CASE WHEN O.[DateTo] = A.[DateTo] THEN NULL ELSE A.[DateTo] END as [DateTo]
      ,CASE WHEN O.[ReminderMinutesBeforeStart] = A.[ReminderMinutesBeforeStart] THEN NULL ELSE A.[ReminderMinutesBeforeStart] END as [ReminderMinutesBeforeStart]
      ,CASE WHEN O.[Notification] = A.[Notification] THEN NULL ELSE A.[Notification] END as [Notification]
      ,CASE WHEN O.[IsPrivate] = A.[IsPrivate] THEN NULL ELSE A.[IsPrivate] END as [IsPrivate]
      ,CASE WHEN O.[Priority] = A.[Priority] THEN NULL ELSE A.[Priority] END as [Priority]
  FROM [AdapterAppointments] A
  LEFT JOIN [OldAdapterAppointments] O ON O.Id = A.Id
  WHERE A.[Id] =  @Id
