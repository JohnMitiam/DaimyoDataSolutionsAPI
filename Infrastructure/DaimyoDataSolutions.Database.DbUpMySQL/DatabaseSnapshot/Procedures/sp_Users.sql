/* sp_CreateUser */
DROP PROCEDURE IF EXISTS `sp_CreateUser`;

DELIMITER $$
CREATE PROCEDURE `sp_CreateUser`(
    IN `UserName` VARCHAR(50),
    IN `Email` VARCHAR(100),
    IN `Phone` INT(20),
    IN `Status` VARCHAR(20),
    IN `IsActive` TINYINT(1),
    IN `CreatedBy` VARCHAR(50),
    IN `DateCreated` DATETIME
)
BEGIN
    INSERT INTO Users (
        UserName,
        Email,
        Phone,
        Status,
        IsActive, 
        CreatedBy, 
        DateCreated,
        IsDeleted
    ) 
    VALUES (
        UserName,
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

/* sp_DeleteUser */
DROP PROCEDURE IF EXISTS `sp_DeleteUser`;

DELIMITER $$
CREATE  PROCEDURE `sp_DeleteUser`(
	IN `UserID` INT
    )
BEGIN
	DELETE FROM Users
		WHERE ID = UserID;
END$$

DELIMITER ;

/* sp_GetUserByID */
DROP PROCEDURE IF EXISTS `sp_GetUserById`;

DELIMITER $$
CREATE PROCEDURE `sp_GetUserById`(
    IN `UserId` INT
)
BEGIN
	SELECT 
            ID,
            UserName,
            Email,
            Phone,
            Status, 
            IsActive, 
            CreatedBy, 
            DateCreated,
            DateUpdated
		FROM Users 
        WHERE ID = UserId;
END$$

DELIMITER ;

/* sp_UpdateUser */
DROP PROCEDURE IF EXISTS `sp_UpdateUser`;

DELIMITER $$
CREATE PROCEDURE `sp_UpdateUser`(
    IN `UserID` INT,
    IN `UserName` VARCHAR(50),
    IN `Email` VARCHAR(100),
    IN `Phone` INT(20),
    IN `Status` VARCHAR(20),
    IN `IsActive` TINYINT(1),
    IN `UpdatedBy` VARCHAR(50),
    IN `DateUpdated` DATETIME)
BEGIN
    UPDATE Users SET
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        Status = Status,
        IsActive = IsActive,
        UpdatedBy = UpdatedBy,
        DateUpdated = DateUpdated
    WHERE ID = UserID;
END$$

DELIMETER ;