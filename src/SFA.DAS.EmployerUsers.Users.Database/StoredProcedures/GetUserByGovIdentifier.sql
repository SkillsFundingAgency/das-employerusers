CREATE PROCEDURE GetUserByGovIdentifier
	@UserId varchar(150)
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
	WHERE GovUkIdentifier = @UserId
GO