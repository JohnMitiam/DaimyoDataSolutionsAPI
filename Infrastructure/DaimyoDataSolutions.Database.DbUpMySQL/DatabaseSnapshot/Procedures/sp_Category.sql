/* sp_CreateCategory */
DROP PROCEDURE IF EXISTS `sp_CreateCategory`;

DELIMITER $$
CREATE PROCEDURE `sp_CreateCategory`(
	IN `Name` VARCHAR(50),
	IN `Description` VARCHAR(100),
	IN `Icon` MEDIUMTEXT,
	IN `CreatedBy` VARCHAR(50),
	IN `DateCreated` DATETIME
	)
BEGIN
	INSERT INTO Category(
		Name, 
		Description,
		Icon,
		CreatedBy,
		DateCreated,
		IsDeleted
		) 
	VALUES (
		Name,
		Description,
		Icon,
		Price,
		IsActive,
		CreatedBy,
		DateCreated,
		0
	);

	SELECT LAST_INSERT_ID();
END$$

DELIMITER ;



/* sp_GetCategoryId */
DROP PROCEDURE IF EXISTS `sp_GetCategoryId`;

DELIMITER $$
CREATE  PROCEDURE `sp_GetCategoryById`(
	IN `CategoryID` INT
)
BEGIN
	SELECT 
            ID,
            Name,
            Description,
			Icon
		FROM Category 
        WHERE ID = CategoryID;
END$$

DELIMITER ;


/* sp_UpdateCategory */
DROP PROCEDURE IF EXISTS `sp_UpdateCategory`;

DELIMITER $$
CREATE  PROCEDURE `sp_UpdateCategory`(
		IN `Name` VARCHAR(50),
		IN `Description` VARCHAR(100),
		IN `Icon` MEDIUMTEXT,
		IN `UpdatedBy` VARCHAR(50),
		IN `DateUpdated` DATETIME
		IN `CategoryID` INT
)
BEGIN
		UPDATE Category SET
			Name = Name,
            Description = Description,
			Icon = Icon,
			UpdatedBy = UpdatedBy,
			DateUpdated = DateUpdated
		WHERE ID = CategoryID;
END$$

DELIMITER ;

/* sp_DeleteCategory */
DROP PROCEDURE IF EXISTS `sp_DeleteCategory`;

DELIMITER $$
CREATE PROCEDURE `sp_DeleteCategory`(
	IN `CategoryID` INT
)
BEGIN
	DELETE FROM Category
		WHERE ID = CategoryID;
END$$

DELIMITER ;