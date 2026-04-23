CREATE  PROCEDURE `sp_GetCategoryById`(
	IN `ID` INT
)
BEGIN
	SELECT 
            ID,
            Name,
            Description,
			Icon
		FROM Category 
        WHERE ID = ID;
END