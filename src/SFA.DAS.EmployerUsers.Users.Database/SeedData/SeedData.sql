/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
IF NOT EXISTS(Select 1 from dbo.RelyingParty where Id='testrp' and ApplicationUrl='http://localhost:17995/')
BEGIN
	INSERT INTO dbo.RelyingParty (Id,Name,RequireConsent,ApplicationUrl,LogoutUrl,Flow,ClientSecret)
	VALUES ('testrp','Test relying party',0,'http://localhost:17995/','http://localhost:17995/',0,'rsgISEW0GmlS1Gy6ocm3mGWUh//RM3ltldBbpF2QlsI=')
END

IF NOT EXISTS(Select 1 from dbo.RelyingParty where Id='employerportal' and ApplicationUrl='http://localhost:58887/')
BEGIN
	INSERT INTO dbo.RelyingParty (Id,Name,RequireConsent,ApplicationUrl,LogoutUrl,Flow,ClientSecret)
	VALUES ('employerportal','Employer Portal',0,'http://localhost:58887/','http://localhost:58887/',1,NULL)
END