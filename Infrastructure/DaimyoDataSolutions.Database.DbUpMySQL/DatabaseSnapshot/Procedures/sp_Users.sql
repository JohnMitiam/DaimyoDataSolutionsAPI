DROP PROCEDURE IF EXISTS `sp_CreateUser`;
DROP PROCEDURE IF EXISTS `sp_DeleteUser`;
DROP PROCEDURE IF EXISTS `sp_GetUserById`;
DROP PROCEDURE IF EXISTS `sp_UpdateUser`;

DELIMITER $$

--- sp_CreateUser
CREATE  PROCEDURE `sp_CreateUser`(
	IN UserName VARCHAR(50),
    IN Email VARCHAR(100),
    IN Status VARCHAR(20),
    IN IsActive TINYINT(1),
    IN CreatedBy VARCHAR(100),
    IN DateCreated DATETIME
    )
BEGIN
		INSERT INTO Users(
            UserName,
            Email,
            Status,
            IsActive, 
            CreatedBy, 
            DateCreated,
            IsDeleted) 
        VALUES (
            UserName,
            Email,
            Status,
            IsActive,
            CreatedBy,
            DateCreated,
            0);

        SELECT LAST_INSERT_ID();
END$$


--- sp_DeleteUser
CREATE  PROCEDURE `sp_DeleteUser`(
	IN `UserID` INT
    )
BEGIN
	DELETE FROM Users
		WHERE ID = UserID;
END$$

--- sp_GetUserByID
CREATE PROCEDURE `sp_GetUserById`(
    IN `UserId` INT
)
BEGIN
	SELECT 
            ID,
            UserName,
            Email,
            Status, 
            IsActive, 
            CreatedBy, 
            DateCreated,
            DateUpdated
		FROM Users 
        WHERE ID = UserId;
END$$


--- sp_UpdateUser
CREATE PROCEDURE `sp_UpdateUser`(
    IN `UserID` INT,
    IN `UserName` VARCHAR(50),
    IN `Email` VARCHAR(100),
    IN `Status` VARCHAR(20),
    IN `IsActive` TINYINT(1),
    IN `CreatedBy` VARCHAR(100),
    IN `DateUpdated` DATETIME
)
BEGIN
    UPDATE Users SET
        UserName = UserName,
        Email = Email,
        Status = Status,
        IsActive = IsActive,
        CreatedBy = CreatedBy,
        DateUpdated = DateUpdated
    WHERE ID = UserID;
END$$

DELIMETER;