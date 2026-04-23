CREATE  PROCEDURE `sp_DeleteAffiliate`(
	IN `ID` INT
    )
BEGIN
	DELETE FROM Affiliate
		WHERE ID = ID;
END