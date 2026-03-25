CREATE TABLE `Products` (
    `Id` INT NOT NULL AUTO_INCREMENT,
    `Name` VARCHAR(100) NOT NULL,
    `Description` VARCHAR(200) NULL,
    `Price` DECIMAL(18,2) NULL,
    `IsActive` TINYINT(1) NULL DEFAULT TRUE,
    `DateCreated` DATETIME NOT NULL,
    `DateUpdated` DATETIME NULL,
    PRIMARY KEY (`Id`)
);

