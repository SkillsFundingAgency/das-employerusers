INSERT INTO dbo.RelyingParty (Id,Name,RequireConsent,ApplicationUrl,LogoutUrl,Flow,ClientSecret)
VALUES ('testrp','Test relying party',0,'http://localhost:17995/','http://localhost:17995/',1,'rsgISEW0GmlS1Gy6ocm3mGWUh//RM3ltldBbpF2QlsI=')
GO

INSERT INTO dbo.RelyingParty (Id,Name,RequireConsent,ApplicationUrl,LogoutUrl,Flow,ClientSecret)
VALUES ('employerportal','Employer Portal',0,'http://localhost:58887/','http://localhost:58887/',1,NULL)
GO