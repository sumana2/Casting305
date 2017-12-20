
CREATE TABLE CastingLand.Talent (
	ID        			INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	FirstName   		VARCHAR(100) NOT NULL,
	LastName   			VARCHAR(100) NOT NULL,
	Gender   			VARCHAR(100) NULL,
	DateOfBirth   		DATETIME NULL,
	Nationality   		VARCHAR(100) NULL,
	Representative   	INT NULL,
	Height   			DECIMAL(16,6) NULL,
	EyeColor   			VARCHAR(100) NULL,
	HairColor   		VARCHAR(100) NULL,
	Ethnicity   		VARCHAR(100) NULL,
	ShoeSize   			VARCHAR(25) NULL,
	WaistSize   		VARCHAR(25) NULL,
	ShirtSize   		VARCHAR(25) NULL,
	Instagram   		VARCHAR(100) NULL,
	Phone   			VARCHAR(20) NULL,
	Email   			VARCHAR(100) NULL,
	Notes   			VARCHAR(5000) NULL,
	ProfilePicture   	VARCHAR(500) NULL
)