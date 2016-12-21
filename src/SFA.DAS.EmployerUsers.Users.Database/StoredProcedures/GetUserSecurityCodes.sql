CREATE PROCEDURE GetUserSecurityCodes
	@UserId varchar(50)
AS
	SELECT
		Code,
		CodeType,
		ExpiryTime,
		ReturnUrl,
		PendingValue
	FROM UserSecurityCode
	WHERE UserId = @UserId
GO