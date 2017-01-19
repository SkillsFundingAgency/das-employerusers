CREATE PROCEDURE [dbo].[DeleteUser]
	@Id varchar(50)
AS
	DELETE FROM [dbo].[UserSecurityCode] WHERE UserId = @Id
	DELETE FROM [dbo].[UserPasswordHistory] WHERE UserId = @Id
	DELETE FROM [dbo].[User] WHERE Id = @Id
