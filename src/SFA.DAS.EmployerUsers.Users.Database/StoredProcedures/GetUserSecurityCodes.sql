CREATE PROCEDURE GetUserSecurityCodes
	@UserId varchar(50)
AS
	SELECT
		Code,
		CodeType,
		ExpiryTime,
		ReturnUrl,
		PendingValue,
		FailedAttempts
	FROM UserSecurityCode
	WHERE UserId = @UserId
GO