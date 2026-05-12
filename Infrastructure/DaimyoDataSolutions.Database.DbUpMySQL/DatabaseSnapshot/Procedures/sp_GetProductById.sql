CREATE PROCEDURE `sp_GetProductById`(IN p_ID INT)
BEGIN
    SELECT 
        p.*, 
        -- ProductCategories Mapping
        pc.Id AS ProductCategoryId, -- Marker 1: For splitOn
        pc.Id,                      -- For ProductCategories.Id property
        pc.ProductId,
        pc.CategoryId,
        pc.CategoryName,            -- Matches your table/entity update
        
        -- Category Mapping
        c.Id AS CategoryId,         -- Marker 2: For splitOn
        c.Id,                       -- For Category.Id property
        c.Name,
        c.Icon,

        -- ProductImages Mapping
        pi.Id AS ProductImageId,    -- Marker 3: For splitOn
        pi.Id,                      -- For ProductImages.Id property
        pi.ImageData,
        pi.MimeType,
        pi.IsPrimary
    FROM Products p
    LEFT JOIN ProductCategories pc ON p.Id = pc.ProductId AND pc.IsDeleted = 0
    LEFT JOIN Category c ON pc.CategoryId = c.Id 
    LEFT JOIN ProductImages pi ON p.Id = pi.ProductId AND pi.IsDeleted = 0
    WHERE p.Id = p_ID AND p.IsDeleted = 0;
END