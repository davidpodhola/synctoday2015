--declare @id int = 5
SELECT * FROM ProductSoldToCustomers where isnull(@id, id) = id
