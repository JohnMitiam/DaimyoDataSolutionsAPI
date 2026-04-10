CREATE TABLE `Category` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Name` varchar(50) NOT NULL,
  `Description` varchar(100) NULL,
  `Icon` mediumtext,
  `CreatedBy` varchar(50) NULL,
  `DateCreated` datetime NOT NULL,
  `UpdatedBy` varchar(50) DEFAULT NULL,
  `DateUpdated` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`Id`),
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;