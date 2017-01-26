CREATE PROCEDURE [dbo].[GetUsers]
	@pageSize int,
	@offset int
AS
		SELECT
		Id,
		FirstName,
		LastName,
		Email,

		PasswordProfileId,
		IsActive,
		FailedLoginAttempts,
		IsLocked
	FROM dbo.[User]
	ORDER BY ID
	OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY
GO
