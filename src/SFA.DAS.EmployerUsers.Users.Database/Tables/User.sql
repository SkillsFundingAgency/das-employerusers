CREATE TABLE dbo.[User] (
	[Id]					varchar(50)		NOT NULL,
	[FirstName]				nvarchar(50)	NULL,
	[LastName]				nvarchar(50)	NULL,
	[Email]					varchar(255)	NOT NULL,
	[Password]				varchar(2048)	NULL,
	[Salt]					varchar(2048)	NULL,
	[PasswordProfileId]		varchar(50)		NULL,
	[IsActive]				bit				NOT NULL DEFAULT(1),
	[FailedLoginAttempts]	int				NOT NULL DEFAULT(0),
	[IsLocked]				bit				NOT NULL DEFAULT(0),
	[IsSuspended]			bit				NOT NULL DEFAULT(0),
    [GovUkIdentifier]       varchar(150)    NULL
	CONSTRAINT [PK_User] PRIMARY KEY (Id), 
    [LastSuspendedDate] DATETIME NULL,
	INDEX [IX_User_Email] NONCLUSTERED(Email),
    INDEX [IX_User_GovUkIdentifier] NONCLUSTERED(GovUkIdentifier)
)