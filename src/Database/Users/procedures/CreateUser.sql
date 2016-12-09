CREATE PROCEDURE CreateUser
	@Id varchar(50),
	@FirstName nvarchar(50),
	@LastName nvarchar(50),
	@Email varchar(255),
	@Password varchar(2048),
	@Salt varchar(2048),
	@PasswordProfileId varchar(50),
	@IsActive bit,
	@FailedLoginAttempts int,
	@IsLocked bit
AS
	INSERT INTO [User]
	(Id, FirstName, LastName, Email, Password, Salt, PasswordProfileId, IsActive, FailedLoginAttempts, IsLocked)
	VALUES
	(@Id, @FirstName, @LastName, @Email, @Password, @Salt, @PasswordProfileId, @IsActive, @FailedLoginAttempts, @IsLocked)
GO