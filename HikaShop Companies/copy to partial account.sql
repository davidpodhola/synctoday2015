USE [SyncToday2015]
GO

begin tran

INSERT INTO [dbo].[adapters.hikashop.PartialAccounts]
           (PartialAccountId
           ,[name]
           ,[ic]
           ,[dic]
           ,[externalid]
           ,[email]
           ,[PrimaryPhonenumber]
           ,[City]
           ,[Street]
           ,[Region]
           ,[Postcode]
           ,[Country]
		   , AdapterId
           )

-- osoby
select newid(),
      [address_firstname] + ' ' + [address_lastname], --,[name]
           NULL, --,[ico]
           address_vat, --,[dic]
           address_id, --,[externalid]
           (select [user_email] from [dbo].[adapters.hikashop.User] U where U.user_id = address_user_id ),--[email]
           address_telephone, --,[PrimaryPhonenumber]
           address_city, --,[City]
           address_street, --,[Street]
           address_state, --,[Region]
           address_post_code, --,[Postcode]
           address_country --,[Country]
		   , '18077449-4CB9-4FE2-BE4C-B5AD34AD5AC2'
FROM [dbo].[adapters.hikashop.Address]
where ( address_company is null or LEN(address_company) = 0 )
and address_id not in (  select externalid from  [adapters.hikashop.PartialAccounts])
and [address_firstname] is not null
and [address_lastname] is not null
and LEN([address_firstname] + ' ' + [address_lastname])>1

UNION


select newid(),
           address_company, --,[name]
           NULL, --,[ico]
           address_vat, --,[dic]
           address_id, --,[externalid]
           (select [user_email] from [dbo].[adapters.hikashop.User] U where U.user_id = address_user_id ),--[email]
           address_telephone, --,[PrimaryPhonenumber]
           address_city, --,[City]
           address_street, --,[Street]
           address_state, --,[Region]
           address_post_code, --,[Postcode]
           address_country --,[Country]
		   , '18077449-4CB9-4FE2-BE4C-B5AD34AD5AC2'
FROM [dbo].[adapters.hikashop.Address]
where address_company is not null and LEN(address_company) > 0
and address_id not in (  select externalid from  [adapters.hikashop.PartialAccounts])

commit

