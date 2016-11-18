CREATE PROCEDURE CreateUserSecurityCode
	@Code varchar(50),
	@UserId varchar(50),
	@CodeType int,
	@ExpiryTime datetime,
	@ReturnUrl varchar(max)
AS
	IF NOT EXISTS (SELECT Code FROM UserSecurityCode WHERE Code = @Code AND UserId = @UserId)
		BEGIN
			INSERT INTO UserSecurityCode
			(Code, UserId, CodeType, ExpiryTime, ReturnUrl)
			VALUES
			(@Code, @UserId, @CodeType, @ExpiryTime, @ReturnUrl)
		END
GO