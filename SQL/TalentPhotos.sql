
CREATE TABLE CastingLand.TalentPhotos (
	ID        	INT NOT NULL PRIMARY KEY AUTO_INCREMENT,
	TalentID	INT NOT NULL,
	PhotoType   VARCHAR(25),
	Photo   	VARCHAR(500),
	Thumbnail   VARCHAR(500)
)
	

