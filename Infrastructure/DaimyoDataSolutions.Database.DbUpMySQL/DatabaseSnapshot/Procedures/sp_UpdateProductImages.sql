CREATE PROCEDURE `sp_UpdateProductImages`(
	IN `ProductId` INT,
	IN `ImageData` LONGBLOB,
    IN `MimeType` LONGTEXT,
	IN `IsPrimary` TINYINT(1),
	IN `UpdatedBy` LONGTEXT,
	IN `DateCreated` DATETIME,
	IN `IsDeleted` TINYINT(1)
	)
BEGIN
	UPDATE ProductImages SET
		ProductId = ProductId,
		ImageData = ImageData,
		MimeType = MimeType,
		IsPrimary = IsPrimary,
		UpdatedBy = UpdatedBy,
			DateUpdated = DateUpdated
		WHERE ID = ID;
END