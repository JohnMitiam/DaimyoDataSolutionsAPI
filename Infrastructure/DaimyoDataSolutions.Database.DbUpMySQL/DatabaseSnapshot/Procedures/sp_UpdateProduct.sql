CREATE  PROCEDURE `sp_UpdateProduct`(
		IN `Name` VARCHAR(255),
        IN `Price` DECIMAL(65,30),
		IN `Description` LONGTEXT,
		IN `IsActive` TINYINT(1),
		IN `UpdatedBy` LONGTEXT,
		IN `DateUpdated` DATETIME,
		IN `ID` INT
)
BEGIN
		UPDATE Products SET
			Name = Name,
            Price = Price,
            Description = Description,
			IsActive = IsActive,
			UpdatedBy = UpdatedBy,
			DateUpdated = DateUpdated
		WHERE ID = ID;
END
