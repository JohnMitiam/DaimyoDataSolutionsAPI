CREATE PROCEDURE `sp_GetProductById`(IN p_ID INT)
BEGIN
    SELECT 
        p.*,                          -- Everything for the 'Products' object
        pc.Id AS ProductCategoryId,   -- Marker 1: This column or 'ProductId' must exist
        pc.ProductId, 
        pc.CategoryId,
        c.Id AS CategoryId,           -- Marker 2: Start of the 'Category' object
        c.Name, 
        c.Icon
    FROM Products p
    LEFT JOIN ProductCategories pc ON p.Id = pc.ProductId AND pc.IsDeleted = 0
    LEFT JOIN Category c ON pc.CategoryId = c.Id
    WHERE p.Id = p_ID AND p.IsDeleted = 0;
END