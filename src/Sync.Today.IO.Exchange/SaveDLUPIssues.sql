declare @LastDLError nvarchar(max) = @LastDLErrorVal
declare @LastUPError nvarchar(max) = @LastUPErrorVal
declare @externalId nvarchar(max) = @externalIdVal



BEGIN TRAN 

UPDATE [dbo].[ExchangeAppointments] 
SET [LastDLError] = @LastDLError, [LastUPError] = @LastUPError
WHERE externalId = @externalId

IF @@ROWCOUNT = 0
BEGIN
  INSERT [ExchangeAppointments] (
	   [LastDLError]
      ,[LastUPError]
      ,externalId
		   ) 
   SELECT @LastDLError, @LastUPError, @ExternalId;
END

COMMIT

SELECT * FROM [ExchangeAppointments]
	WHERE externalId = @externalId
 