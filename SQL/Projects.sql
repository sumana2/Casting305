IF NOT EXISTS (SELECT 1 FROM SysObjects WHERE id=Object_ID('dbo.Projects') AND ObjectProperty(id,'IsUserTable')=1)
BEGIN

	CREATE TABLE dbo.Projects (
		ID        			INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		Title   		    VARCHAR(100) NOT NULL,
		Company   			VARCHAR(100) NULL,
		Email   			VARCHAR(25) NULL,
		Phone   		    VARCHAR(25) NULL,
		DueDate   		    DATETIME NULL
	)
	
	PRINT 'Created table: [Projects]'

END
GO
