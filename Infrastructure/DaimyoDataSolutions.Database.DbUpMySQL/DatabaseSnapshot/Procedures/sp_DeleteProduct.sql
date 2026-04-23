CREATE PROCEDURE `sp_DeleteProduct`(
	IN `ProductID` INT
)
BEGIN
	DELETE FROM Products
		WHERE ID = ProductID;
END