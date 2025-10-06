CREATE PROCEDURE [dbo].[GetUsersWithExpiredRegistrations]
AS
	SELECT
		Id,
		FirstName,
		LastName,
		Email,
		Password,
		Salt,
		PasswordProfileId,
		IsActive,
		FailedLoginAttempts,
		IsLocked
	FROM [User]
	WHERE IsActive = 0
	AND Id NOT IN
	(
		SELECT UserId
		FROM UserSecurityCode
		WHERE ExpiryTime >= GETDATE()
	)
GO