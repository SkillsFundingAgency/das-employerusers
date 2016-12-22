CREATE PROCEDURE CreateUserSecurityCode
	@Code varchar(50),
	@UserId varchar(50),
	@CodeType int,
	@ExpiryTime datetime,
	@ReturnUrl varchar(max),
	@PendingValue nvarchar(255)
AS
	INSERT INTO UserSecurityCode
	(Code, UserId, CodeType, ExpiryTime, ReturnUrl, PendingValue)
	VALUES
	(@Code, @UserId, @CodeType, @ExpiryTime, @ReturnUrl, @PendingValue)
GO