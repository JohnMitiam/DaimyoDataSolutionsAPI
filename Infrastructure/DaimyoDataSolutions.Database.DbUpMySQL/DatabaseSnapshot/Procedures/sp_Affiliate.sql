/* sp_CreateAffiliate */
DROP PROCEDURE IF EXISTS `sp_CreateAffiliate`;

DELIMITER $$
CREATE PROCEDURE `sp_CreateAffiliate`(
    IN `Name` VARCHAR(50),
    IN `Email` VARCHAR(100),
    IN `Phone` VARCHAR(15),
    IN `Status` VARCHAR(20),
    IN `IsActive` TINYINT(1),
    IN `CreatedBy` VARCHAR(50),
    IN `DateCreated` DATETIME
)
BEGIN
    INSERT INTO Affiliate (
        AffiliateName,
        Email,
        Phone,
        Status,
        IsActive, 
        CreatedBy, 
        DateCreated,
        IsDeleted
    ) 
    VALUES (
        AffiliateName,
        Email,
        Phone,
        Status,
        IsActive,
        CreatedBy,
        DateCreated,
        0
    );

    SELECT LAST_INSERT_ID();
END$$

DELIMITER ;

/* sp_DeleteAffiliate */
DROP PROCEDURE IF EXISTS `sp_DeleteAffiliate`;

DELIMITER $$
CREATE  PROCEDURE `sp_DeleteAffiliate`(
	IN `ID` INT
    )
BEGIN
	DELETE FROM Affiliate
		WHERE ID = ID;
END$$

DELIMITER ;

/* sp_GetAffiliateByID */
DROP PROCEDURE IF EXISTS `sp_GetAffiliateById`;

DELIMITER $$
CREATE PROCEDURE `sp_GetAffiliateById`(
    IN `ID` INT
)
BEGIN
	SELECT 
            ID,
            Name,
            Email,
            Phone,
            Status, 
            IsActive, 
            CreatedBy, 
            DateCreated,
            DateUpdated
		FROM Affiliate 
        WHERE ID = ID;
END$$

DELIMITER ;

/* sp_UpdateAffiliate */
DROP PROCEDURE IF EXISTS `sp_UpdateAffiliate`;

DELIMITER $$
CREATE PROCEDURE `sp_UpdateAffiliate`(
    IN `ID` INT,
    IN `Name` VARCHAR(50),
    IN `Email` VARCHAR(100),
    IN `Phone` INT(20),
    IN `Status` VARCHAR(20),
    IN `IsActive` TINYINT(1),
    IN `UpdatedBy` VARCHAR(50),
    IN `DateUpdated` DATETIME)
BEGIN
    UPDATE Affiliate SET
        Name = Name,
        Email = Email,
        Phone = Phone,
        Status = Status,
        IsActive = IsActive,
        UpdatedBy = UpdatedBy,
        DateUpdated = DateUpdated
    WHERE ID = ID;
END$$

DELIMETER ;