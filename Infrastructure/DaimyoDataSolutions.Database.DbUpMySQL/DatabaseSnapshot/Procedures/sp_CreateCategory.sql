CREATE PROCEDURE `sp_CreateCategory`(
	IN `Name` VARCHAR(255),
	IN `Description` LONGTEXT,
	IN `Icon` LONGTEXT,
	IN `CreatedBy` LONGTEXT,
	IN `DateCreated` DATETIME,
	IN `IsDeleted` TINYINT(1)
	)
BEGIN
	INSERT INTO Category(
		Name, 
		Description,
		Icon,
		CreatedBy,
		DateCreated,
		IsDeleted
		) 
	VALUES (
		Name,
		Description,
		Icon,
		CreatedBy,
		DateCreated,
		0
	);

	SELECT LAST_INSERT_ID();
END