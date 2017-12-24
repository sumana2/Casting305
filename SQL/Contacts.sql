
CREATE TABLE CastingLand.Contacts (
	ID        		INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	SourceID        INT NOT NULL,
	Type            VARCHAR(100) NOT NULL,
	FirstName   	VARCHAR(100) NOT NULL,
	LastName   		VARCHAR(100) NOT NULL,
	Email   		VARCHAR(100) NULL,
	Phone           VARCHAR(20) NULL
)