INSERT INTO dbo.RelyingParty (Id,Name,RequireConsent,ApplicationUrl,LogoutUrl)
VALUES ('testrp','Test relying party',0,'http://localhost:17995/','http://localhost:17995/')
GO

INSERT INTO dbo.RelyingParty (Id,Name,RequireConsent,ApplicationUrl,LogoutUrl)
VALUES ('employerportal','Employer Portal',0,'http://localhost:58887/','http://localhost:58887/')
GO