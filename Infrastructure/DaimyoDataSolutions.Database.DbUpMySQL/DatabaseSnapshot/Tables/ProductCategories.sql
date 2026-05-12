CREATE TABLE `ProductCategories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProductId` int NOT NULL,
  `CategoryId` int DEFAULT NULL,
  `CreatedBy` varchar(50) NULL,
  `DateCreated` datetime NOT NULL,
  `UpdatedBy` varchar(50) DEFAULT NULL,
  `DateUpdated` datetime DEFAULT NULL,
  `IsDeleted` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`Id`),
) ENGINE=InnoDB AUTO_INCREMENT=64 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
