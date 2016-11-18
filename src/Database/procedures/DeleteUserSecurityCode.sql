CREATE PROCEDURE DeleteUserSecurityCode
	@Code varchar(50)
AS
	DELETE FROM UserSecurityCode
	WHERE Code = @Code
GO