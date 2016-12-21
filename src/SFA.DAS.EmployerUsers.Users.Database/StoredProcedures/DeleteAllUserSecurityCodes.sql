CREATE PROCEDURE [dbo].[DeleteAllUserSecurityCodes]
	@Userid varchar(50)
AS
	DELETE FROM [UserSecurityCode] WHERE UserId = @UserId
GO