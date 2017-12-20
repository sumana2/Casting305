
CREATE TABLE CastingLand.Contacts (
	ID        		INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	ClientID        INT,
	FirstName   	VARCHAR(100) NOT NULL,
	LastName   		VARCHAR(100) NOT NULL,
	Email   		VARCHAR(100) NULL,
	Phone           VARCHAR(20) NULL
)