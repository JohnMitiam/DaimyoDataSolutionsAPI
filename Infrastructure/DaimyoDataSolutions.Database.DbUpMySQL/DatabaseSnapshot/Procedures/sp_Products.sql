/* sp_CreateProduct */
DROP PROCEDURE IF EXISTS `sp_CreateProduct`;

DELIMITER $$
CREATE PROCEDURE `sp_CreateProduct`(
	IN `Name` VARCHAR(100),
	IN `Description` VARCHAR(150),
	IN `Price` DECIMAL(20,0),
	IN `IsActive` TINYINT(1),
	IN `CreatedBy` VARCHAR(50),
	IN `DateCreated` DATETIME
	)
BEGIN
	INSERT INTO Products(
		Name, 
		Description, 
		Price, 
		IsActive,
		CreatedBy,
		DateCreated,
		IsDeleted
		) 
	VALUES (
		Name,
		Description,
		Price,
		IsActive,
		CreatedBy,
		DateCreated,
		0
	);

	SELECT LAST_INSERT_ID();
END$$

DELIMITER ;



/* sp_GetProductId */
DROP PROCEDURE IF EXISTS `sp_GetProductId`;

DELIMITER $$
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
END$$

DELIMITER ;


/* sp_UpdateProduct */
DROP PROCEDURE IF EXISTS `sp_UpdateProduct`;

DELIMITER $$
CREATE  PROCEDURE `sp_UpdateProduct`(
		IN `Name` VARCHAR(100),
        IN `Price` DECIMAL(20,0),
		IN `Description` VARCHAR(150),
		IN `IsActive` TINYINT(1),
		IN `UpdatedBy` VARCHAR(50),
		IN `DateUpdated` DATETIME,
		IN `ID` INT
)
BEGIN
		UPDATE Products SET
			Name = Name,
            Price = Price,
            Description = Description,
			IsActive = IsActive,
			UpdatedBy = UpdatedBy,
			DateUpdated = DateUpdated
		WHERE ID = ID;
END$$

DELIMITER ;

/* sp_DeleteProduct */
DROP PROCEDURE IF EXISTS `sp_DeleteProduct`;

DELIMITER $$
CREATE PROCEDURE `sp_DeleteProduct`(
	IN `ID` INT
)
BEGIN
	DELETE FROM Products
		WHERE ID = ID;
END$$

DELIMITER ;