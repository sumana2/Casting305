IF NOT EXISTS (SELECT 1 FROM SysObjects WHERE id=Object_ID('dbo.ProjectRoles') AND ObjectProperty(id,'IsUserTable')=1)
BEGIN

	CREATE TABLE dbo.ProjectRoles (
		ID        			INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		ProjectID   		INT NOT NULL,
		Name   				VARCHAR(100) NULL,
		AgeMin   			INT NULL,
		AgeMax   		    INT NULL,
		HeightMin			DECIMAL(16,6) NULL,
		HeightMax			DECIMAL(16,6) NULL,
		HairColor			VARCHAR(100) NULL
	)
	
	PRINT 'Created table: [ProjectRoles]'

END
GO
