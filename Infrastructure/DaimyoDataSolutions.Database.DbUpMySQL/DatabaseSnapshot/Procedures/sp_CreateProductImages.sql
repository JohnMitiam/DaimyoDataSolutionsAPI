CREATE PROCEDURE `sp_CreateProductImages`(
	IN `ProductId` INT,
	IN `ImageData` LONGBLOB,
    IN `MimeType` LONGTEXT,
	IN `IsPrimary` TINYINT(1),
	IN `CreatedBy` LONGTEXT,
	IN `DateCreated` DATETIME,
	IN `IsDeleted` TINYINT(1)
	)
BEGIN
	INSERT INTO ProductImages(
		ProductId, 
		ImageData,
		MimeType,
		IsPrimary,
		CreatedBy,
		DateCreated,
		IsDeleted
		) 
	VALUES (
		ProductId,
		ImageData,
		MimeType,
		IsPrimary,
		CreatedBy,
		DateCreated,
		0
	);

	SELECT LAST_INSERT_ID();
END