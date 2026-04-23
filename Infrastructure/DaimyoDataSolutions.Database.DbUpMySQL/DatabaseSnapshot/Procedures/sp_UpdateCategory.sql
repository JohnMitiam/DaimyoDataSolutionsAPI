CREATE  PROCEDURE `sp_UpdateCategory`(
		IN `Name` VARCHAR(255),
		IN `Description` LONGTEXT,
		IN `Icon` MEDIUMTEXT,
		IN `UpdatedBy` LONGTEXT,
		IN `DateUpdated` DATETIME,
		IN `ID` INT
)
BEGIN
		UPDATE Category SET
			Name = Name,
            Description = Description,
			Icon = Icon,
			UpdatedBy = UpdatedBy,
			DateUpdated = DateUpdated
		WHERE ID = ID;
END