CREATE PROCEDURE DeleteHistoricalPassword
	@UserId varchar(50),
	@Password varchar(2048)
AS
	DELETE FROM UserPasswordHistory
	WHERE UserId = @UserId
	AND Password = @Password
GO