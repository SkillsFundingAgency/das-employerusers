CREATE TABLE dbo.PasswordProfile (
	[Id]			varchar(50)		NOT NULL,
	[Key]			varchar(255)	NOT NULL,
	[WorkFactor]	int				NOT NULL,
	[SaltLength]	int				NOT NULL DEFAULT(15),
	[StorageLength]	int				NOT NULL DEFAULT(256),
	CONSTRAINT [PK_PasswordProfile] PRIMARY KEY (Id)
)