CREATE PROCEDURE GetUserPasswordHistory
	@UserId varchar(50)
AS
	SELECT
		Password,
		Salt,
		PasswordProfileId,
		DateSet
	FROM UserPasswordHistory
GO