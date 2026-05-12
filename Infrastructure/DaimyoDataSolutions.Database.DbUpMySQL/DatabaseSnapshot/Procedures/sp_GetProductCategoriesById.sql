CREATE  PROCEDURE `sp_GetProductCategoriesById`(
	IN `ID` INT
)
BEGIN
	SELECT 
            ID,
            ProductId,
            CateegoryId,
            IsActive
		FROM ProductCategories
        WHERE ID = ID;
END