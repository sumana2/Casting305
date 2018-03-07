
CREATE TABLE CastingLand.ProjectRoles (
	ID        			INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	ProjectID   		INT NOT NULL,
	Name   				VARCHAR(100) NULL,
	AgeMin   			INT NULL,
	AgeMax   		    INT NULL,
	HeightMin			DECIMAL(16,6) NULL,
	HeightMax			DECIMAL(16,6) NULL,
	HairColor			VARCHAR(100) NULL
)

ALTER TABLE CastingLand.ProjectRoles ADD COLUMN Rate VARCHAR(100) AFTER Name;
ALTER TABLE CastingLand.ProjectRoles ADD COLUMN Gender VARCHAR(100) AFTER Rate;
ALTER TABLE CastingLand.ProjectRoles CHANGE HairColor EthicApperance VARCHAR(100) NULL;