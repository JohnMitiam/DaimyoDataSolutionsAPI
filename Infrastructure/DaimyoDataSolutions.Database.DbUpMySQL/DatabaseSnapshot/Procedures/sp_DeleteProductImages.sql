CREATE PROCEDURE `sp_DeleteProductImages`(
	IN `ProductImageID` INT
)
BEGIN
	DELETE FROM ProductImages
		WHERE ID = ProductImageID;
END