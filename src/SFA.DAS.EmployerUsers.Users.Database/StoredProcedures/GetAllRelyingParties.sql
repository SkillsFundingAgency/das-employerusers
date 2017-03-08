CREATE PROCEDURE dbo.GetAllRelyingParties
AS
	SELECT
		Id,
		Name,
		RequireConsent,
		ApplicationUrl,
		LogoutUrl,
		LoginCallBackUrl,
		Flow,
		ClientSecret
	FROM dbo.RelyingParty