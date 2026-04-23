CREATE PROCEDURE `sp_CreateProduct`(
	IN `Name` VARCHAR(255),
	IN `Description` LONGTEXT,
	IN `Price` DECIMAL(65,30),
	IN `IsActive` TINYINT(1),
	IN `CreatedBy` LONGTEXT,
	IN `DateCreated` DATETIME,
	IN `IsDeleted` TINYINT(1)
	)
BEGIN
	INSERT INTO Products(
		Name, 
		Description, 
		Price, 
		IsActive,
		CreatedBy,
		DateCreated,
		IsDeleted
		) 
	VALUES (
		Name,
		Description,
		Price,
		IsActive,
		CreatedBy,
		DateCreated,
		0
	);

	SELECT LAST_INSERT_ID();
END