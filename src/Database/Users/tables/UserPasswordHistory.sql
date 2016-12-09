CREATE TABLE UserPasswordHistory
(
	UserId varchar(50) NOT NULL,
	Password varchar(2048) NOT NULL,
	Salt varchar(2048) NOT NULL,
	PasswordProfileId varchar(50) NOT NULL,
	DateSet datetime NOT NULL,
	CONSTRAINT [FK_UserPasswordHistory_User] FOREIGN KEY(UserId) REFERENCES [User](Id)
)
GO