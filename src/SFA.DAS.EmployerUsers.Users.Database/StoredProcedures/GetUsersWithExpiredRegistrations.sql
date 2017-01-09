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
	FROM [User] u
	LEFT JOIN UserSecurityCode sc
		ON u.Id = sc.UserId
		AND sc.ExpiryTime > GETDATE()
	WHERE u.IsActive = 0
	AND sc.Code IS NULL
GO