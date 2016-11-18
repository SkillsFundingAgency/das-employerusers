CREATE PROCEDURE dbo.GetAllRelyingParties
AS
	SELECT
		Id,
		Name,
		RequireConsent,
		ApplicationUrl,
		LogoutUrl
	FROM dbo.RelyingParty