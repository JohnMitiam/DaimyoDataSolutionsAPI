CREATE  PROCEDURE `sp_DeleteAffiliate`(
	IN `AffiliateID` INT
    )
BEGIN
	DELETE FROM Affiliate
		WHERE ID = AffiliateID;
END