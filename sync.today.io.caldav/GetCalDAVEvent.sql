/*
declare @idVal int = 0
declare @ExternalIdVal nvarchar(2048) = NULL
*/

declare @id int = @idVal
declare @ExternalId nvarchar(2048) = @ExternalIdVal

SELECT Id
      ,InternalId
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
  FROM CalDavEvents
  where 
		Id = ( CASE WHEN @id = 0 THEN Id ELSE @id END ) AND
		ExternalId = ISNULL( @ExternalId, ExternalId )