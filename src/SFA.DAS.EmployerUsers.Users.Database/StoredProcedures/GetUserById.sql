CREATE PROCEDURE GetUserById
	@UserId varchar(50)
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
		IsLocked,
		IsSuspended,
		LastSuspendedDate,
		GovUkIdentifier
	FROM dbo.[User]
	WHERE Id = @UserId
GO