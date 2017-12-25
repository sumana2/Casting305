
CREATE TABLE CastingLand.Representatives (
	ID        		INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	Company   		VARCHAR(200) NOT NULL,
	Country   		VARCHAR(100) NULL,
	Email   		VARCHAR(100) NULL,
	Phone   		VARCHAR(20) NULL,
	Address   		VARCHAR(200) NULL,
	BillingInfo   	VARCHAR(500) NULL,
	AdminEmail   	VARCHAR(100) NULL
)
