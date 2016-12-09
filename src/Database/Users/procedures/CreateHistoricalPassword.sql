CREATE PROCEDURE CreateHistoricalPassword
	@UserId varchar(50),
	@Password varchar(2048),
	@Salt varchar(2048),
	@PasswordProfileId varchar(50),
	@DateSet datetime
AS
	INSERT INTO UserPasswordHistory
	(UserId,Password,Salt,PasswordProfileId,DateSet)
	VALUES
	(@UserId,@Password,@Salt,@PasswordProfileId,@DateSet)
GO