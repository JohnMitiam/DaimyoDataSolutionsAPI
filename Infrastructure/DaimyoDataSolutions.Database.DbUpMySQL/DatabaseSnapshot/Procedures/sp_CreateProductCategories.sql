CREATE PROCEDURE `sp_CreateProductCategories`(
	IN `ProductId` INT,
	IN `CategoryId` INT,
	IN `CreatedBy` LONGTEXT,
	IN `DateCreated` DATETIME,
	IN `IsDeleted` TINYINT(1)
	)
BEGIN
	INSERT INTO ProductCategories(
		ProductId, 
		CategoryId,
		CreatedBy,
		DateCreated,
		IsDeleted
		) 
	VALUES (
		ProductId,
		CategoryId,
		CreatedBy,
		DateCreated,
		0
	);

	SELECT LAST_INSERT_ID();
END