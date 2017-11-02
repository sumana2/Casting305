IF NOT EXISTS (SELECT 1 FROM SysObjects WHERE id=Object_ID('dbo.Clients') AND ObjectProperty(id,'IsUserTable')=1)
BEGIN

	CREATE TABLE dbo.Clients (
		ID        		INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		Company   		VARCHAR(200) NOT NULL,
		Country   		VARCHAR(200) NULL,
		Email   		VARCHAR(200) NULL,
		Phone   		VARCHAR(200) NULL,
		Address   		VARCHAR(200) NULL,
		BillingInfo   	VARCHAR(200) NULL,
		AdminEmail   	VARCHAR(200) NULL
	)
	
	PRINT 'Created table: [Clients]'

END
GO
