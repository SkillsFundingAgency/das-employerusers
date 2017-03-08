CREATE PROCEDURE dbo.GetAllRelyingParties
AS
	SELECT
		Id,
		Name,
		RequireConsent,
		ApplicationUrl,
		LogoutUrl,
		LoginUrl,
		Flow,
		ClientSecret
	FROM dbo.RelyingParty