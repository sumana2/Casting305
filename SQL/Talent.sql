IF NOT EXISTS (SELECT 1 FROM SysObjects WHERE id=Object_ID('dbo.Talent') AND ObjectProperty(id,'IsUserTable')=1)
BEGIN

	CREATE TABLE dbo.Talent (
		ID        			INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
		FirstName   		VARCHAR(100) NOT NULL,
		LastName   			VARCHAR(100) NOT NULL,
		Gender   			VARCHAR(25) NULL,
		DateOfBirth   		DATETIME NULL,
		Nationality   		VARCHAR(10) NULL,
		Representative   	INT NULL,
		Height   			DECIMAL(16,6) NULL,
		EyeColor   			VARCHAR(25) NULL,
		HairColor   		VARCHAR(25) NULL,
		Ethnicity   		VARCHAR(25) NULL,
		ShoeSize   			VARCHAR(25) NULL,
		WaistSize   		VARCHAR(25) NULL,
		ShirtSize   		VARCHAR(25) NULL,
		Instagram   		VARCHAR(100) NULL,
		Phone   			VARCHAR(25) NULL,
		Email   			VARCHAR(100) NULL,
		Notes   			VARCHAR(MAX) NULL,
		ProfilePicture   	VARCHAR(200) NULL
	)
	
	PRINT 'Created table: [Talent]'

END
GO
