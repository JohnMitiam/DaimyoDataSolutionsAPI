CREATE PROCEDURE `sp_CreateProductCategories`(
	IN `ProductId` INT,
	IN `CategoryId` INT,
	IN `UpdatedBy` LONGTEXT,
	IN `DateCreated` DATETIME,
	IN `IsDeleted` TINYINT(1)
	)
BEGIN
	UPDATE ProductCategories SET
		ProductId = ProductId,
		CategoryId = CategoryId,
		UpdatedBy = UpdatedBy,
			DateUpdated = DateUpdated
		WHERE ID = ID;
END