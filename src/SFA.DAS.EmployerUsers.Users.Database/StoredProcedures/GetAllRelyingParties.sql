CREATE PROCEDURE dbo.GetAllRelyingParties
AS
	SELECT
		Id,
		Name,
		RequireConsent,
		ApplicationUrl,
		LogoutUrl,
		Flow,
		ClientSecret
	FROM dbo.RelyingParty