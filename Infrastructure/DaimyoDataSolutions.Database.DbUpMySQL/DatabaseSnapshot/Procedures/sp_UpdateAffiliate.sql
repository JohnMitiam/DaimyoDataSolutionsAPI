CREATE PROCEDURE `sp_UpdateAffiliate`(
    IN `ID` INT,
    IN `Name` VARCHAR(255),
    IN `Email` LONGTEXT,
    IN `Phone` LONGTEXT,
    IN `Status` LONGTEXT,
    IN `IsActive` TINYINT(1),
    IN `UpdatedBy` LONGTEXT,
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
END