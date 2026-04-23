CREATE PROCEDURE `sp_DeleteCategory`(
	IN `CategoryID` INT
)
BEGIN
	DELETE FROM Category
		WHERE ID = CategoryID;
END