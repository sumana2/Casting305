IF NOT EXISTS (SELECT 1 FROM SysObjects WHERE id=Object_ID('dbo.Clients') AND ObjectProperty(id,'IsUserTable')=1)
BEGIN

	CREATE TABLE dbo.Contacts (
		ID        		INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		ClientID        INT,
		FirstName   	VARCHAR(100) NOT NULL,
		LastName   		VARCHAR(100) NOT NULL,
		Email   		VARCHAR(100) NULL,
		Phone           VARCHAR(20) NULL
	)
	
	PRINT 'Created table: [Clients]'

END
GO
