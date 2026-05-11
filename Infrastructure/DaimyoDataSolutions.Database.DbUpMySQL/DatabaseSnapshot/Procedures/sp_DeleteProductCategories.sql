CREATE PROCEDURE `sp_DeleteProductCategories`(
	IN `ProductCategoryID` INT
)
BEGIN
	DELETE FROM ProductCategories
		WHERE ID = ProductCategoryID;
END