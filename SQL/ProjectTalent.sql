IF NOT EXISTS (SELECT 1 FROM SysObjects WHERE id=Object_ID('dbo.ProjectTalent') AND ObjectProperty(id,'IsUserTable')=1)
BEGIN

	CREATE TABLE dbo.ProjectTalent (
		ID        			INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		ProjectRoleID   	INT NOT NULL,
		TalentID   			INT NOT NULL
	)
	
	PRINT 'Created table: [ProjectTalent]'

END
GO
