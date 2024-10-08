CREATE TABLE dbo.UserSecurityCode (
	[Code]				varchar(50)		NOT NULL,
	[UserId]			varchar(50)		NOT NULL,
	[CodeType]			int				NOT NULL,
	[ExpiryTime]		datetime		NOT NULL,
	[ReturnUrl]			varchar(max)	NULL,
	[PendingValue]		nvarchar(255)	NULL,
	[FailedAttempts]	int				NOT NULL DEFAULT(0),
	CONSTRAINT [PK_UserSecurityCode] PRIMARY KEY (Code,UserId,CodeType),
	CONSTRAINT [FK_UserSecurityCode_User] FOREIGN KEY (UserId) REFERENCES [User](Id)
)
GO
CREATE NONCLUSTERED INDEX [IX_UserSecurityCode_User] ON UserSecurityCode(UserId)
GO