CREATE  PROCEDURE `sp_GetProductById`(
	IN `ID` INT
)
BEGIN
	SELECT 
            ID,
            Name,
            Description,
            IsActive
		FROM Products 
        WHERE ID = ID;
END