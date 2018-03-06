
CREATE TABLE CastingLand.Projects (
	ID        			INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	Title   		    VARCHAR(100) NOT NULL,
	Company   			VARCHAR(100) NULL,
	Email   			VARCHAR(100) NULL,
	Phone   		    VARCHAR(20) NULL,
	DueDate   		    DATETIME NULL
)

ALTER TABLE CastingLand.Projects ADD COLUMN Notes VARCHAR(1000) AFTER DueDate;
ALTER TABLE CastingLand.Projects ADD COLUMN ProjectType VARCHAR(100) AFTER Notes;
ALTER TABLE CastingLand.Projects ADD COLUMN UsageRun VARCHAR(100) AFTER ProjectType;
ALTER TABLE CastingLand.Projects ADD COLUMN TalentDesc VARCHAR(500) AFTER UsageRun;