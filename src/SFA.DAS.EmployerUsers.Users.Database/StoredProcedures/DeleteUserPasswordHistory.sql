CREATE PROCEDURE [dbo].[DeleteUserPasswordHistory]
	@Userid varchar(50)
AS
	DELETE FROM [UserPasswordHistory] WHERE UserId = @UserId
GO
