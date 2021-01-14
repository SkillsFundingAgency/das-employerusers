CREATE TABLE dbo.[User] (
	[Id]					varchar(50)		NOT NULL,
	[FirstName]				nvarchar(50)	NOT NULL,
	[LastName]				nvarchar(50)	NOT NULL,
	[Email]					varchar(255)	NOT NULL,
	[Password]				varchar(2048)	NOT NULL,
	[Salt]					varchar(2048)	NOT NULL,
	[PasswordProfileId]		varchar(50)		NOT NULL,
	[IsActive]				bit				NOT NULL DEFAULT(1),
	[FailedLoginAttempts]	int				NOT NULL DEFAULT(0),
	[IsLocked]				bit				NOT NULL DEFAULT(0),
	[IsSuspended]			bit				NOT NULL DEFAULT(0)
	CONSTRAINT [PK_User] PRIMARY KEY (Id),
	INDEX [IX_User_Email] NONCLUSTERED(Email)
)