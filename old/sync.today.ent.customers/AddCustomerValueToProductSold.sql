-- uncomment for testing
/*   
DECLARE @productSoldIdVal int = ( select top 1 Id from ProductSoldToCustomers )
DECLARE @productUsageId int = ( select top 1 Id from ProductUsages )
declare @productUsageComment nvarchar(4000) = 'Celkem mu to �lape'
declare @productUsageSuccessLevel decimal = 0.50
*/


DECLARE @productSoldId int = @productSoldIdVal

BEGIN TRAN

UPDATE ProductSoldToCustomers
SET ProductUsageId = @productUsageId, ProductUsageComment = @productUsageComment, ProductUsageSuccessLevel = @productUsageSuccessLevel
WHERE Id = @productSoldId

COMMIT

SELECT * FROM ProductSoldToCustomers WHERE Id = @productSoldId


