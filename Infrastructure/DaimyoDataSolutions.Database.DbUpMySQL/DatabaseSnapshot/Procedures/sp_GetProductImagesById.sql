CREATE  PROCEDURE `sp_GetProductImagesById`(
	IN `ID` INT
)
BEGIN
	SELECT 
            ID,
            ProductId,
			ImageData,
			MimeType,
			IsPrimary
		FROM ProductImages
        WHERE ID = ID;
END