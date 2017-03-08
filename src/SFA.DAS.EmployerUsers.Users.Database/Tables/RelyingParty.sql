CREATE TABLE dbo.RelyingParty (
	[Id] varchar(50) NOT NULL,
	[Name] varchar(125) NOT NULL,
	[RequireConsent] bit NOT NULL DEFAULT(0),
	[ApplicationUrl] varchar(max) NOT NULL,
	[LogoutUrl] varchar(max) NOT NULL,
	[LoginUrl] varchar(max) NULL,
	[Flow] int NOT NULL DEFAULT(1),
	[ClientSecret] nvarchar(255) NULL,
	CONSTRAINT [PK_RelyingParty] PRIMARY KEY (Id)
)