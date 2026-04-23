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
END