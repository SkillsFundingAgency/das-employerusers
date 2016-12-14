CREATE PROCEDURE UpdateUser
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
	UPDATE [User]
	SET FirstName = @FirstName, 
		LastName = @LastName, 
		Email = @Email, 
		Password = @Password, 
		Salt = @Salt, 
		PasswordProfileId = @PasswordProfileId, 
		IsActive = @IsActive, 
		FailedLoginAttempts = @FailedLoginAttempts, 
		IsLocked = @IsLocked
	WHERE Id = @Id
GO