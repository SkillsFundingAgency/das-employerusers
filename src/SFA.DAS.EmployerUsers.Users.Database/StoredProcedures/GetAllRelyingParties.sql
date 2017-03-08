CREATE PROCEDURE dbo.GetAllRelyingParties
AS
	SELECT
		Id,
		Name,
		RequireConsent,
		ApplicationUrl,
		LogoutUrl,
		LoginCallbackUrl,
		Flow,
		ClientSecret
	FROM dbo.RelyingParty