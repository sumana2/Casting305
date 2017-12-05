IF NOT EXISTS (SELECT 1 FROM SysObjects WHERE id=Object_ID('dbo.TalentPhotos') AND ObjectProperty(id,'IsUserTable')=1)
BEGIN

	CREATE TABLE dbo.TalentPhotos (
		ID        	INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		TalentID	INT,
		PhotoType   VARCHAR(25),
		PhotoURL   	VARCHAR(500)
	)
	
	PRINT 'Created table: [TalentPhotos]'

END
GO
