CREATE TABLE dbo.UserSecurityCode (
	[Code]			varchar(50)		NOT NULL,
	[UserId]		varchar(50)		NOT NULL,
	[CodeType]		int				NOT NULL,
	[ExpiryTime]	datetime		NOT NULL,
	[ReturnUrl]		varchar(max)	NULL,
	[PendingValue]	nvarchar(255)	NULL,
	CONSTRAINT [PK_UserSecurityCode] PRIMARY KEY (Code),
	CONSTRAINT [FK_UserSecurityCode_User] FOREIGN KEY (UserId) REFERENCES [User](Id)
)