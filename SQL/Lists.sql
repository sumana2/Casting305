IF NOT EXISTS (SELECT 1 FROM SysObjects WHERE id=Object_ID('dbo.Lists') AND ObjectProperty(id,'IsUserTable')=1)
BEGIN

	CREATE TABLE dbo.Lists (
		ID        	INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		Text   		VARCHAR(100),
		List   		VARCHAR(50)
	)
	
	PRINT 'Created table: [Lists]'

END
GO
