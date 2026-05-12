CREATE  PROCEDURE `sp_GetProductCategoriesById`(
	IN `ID` INT
)
BEGIN
	SELECT 
            ID,
            ProductId,
            CateegoryId
		FROM ProductCategories
        WHERE ID = ID;
END