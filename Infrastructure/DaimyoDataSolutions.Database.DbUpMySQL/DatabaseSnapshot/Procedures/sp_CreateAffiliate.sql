CREATE PROCEDURE `sp_CreateAffiliate`(
    IN `Name` VARCHAR(255),
    IN `Email` LONGTEXT,
    IN `Phone` LONGTEXT,
    IN `Status` LONGTEXT,
    IN `IsActive` TINYINT(1),
    IN `CreatedBy` LONGTEXT,
    IN `DateCreated` DATETIME,
    IN `IsDeleted` TINYINT(1)
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
END