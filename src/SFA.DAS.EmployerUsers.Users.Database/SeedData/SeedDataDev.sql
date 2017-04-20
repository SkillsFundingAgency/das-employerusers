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

IF NOT EXISTS(SELECT TOP 1 * FROM dbo.[User] WHERE Id = '87df36f4-78ad-47c7-84d7-900ef4c39920')
BEGIN
	EXEC [dbo].[CreateUser] '87df36f4-78ad-47c7-84d7-900ef4c39920', 'Test', 'Account', 'test@account.com', 'BjjkHnE7sgbLzLAJLQa7uT2Qv4DpJDElv4tWyJ3iIUT1w7IEXHcvilaIczMuP+30r6lK9/6uX+PcQTfvzfbQAAz7NN1QMYHunrnqd8aSIFMJU5kRHNhGQxoOlDVkCDmf0XR7ePx2EI0B/ItdOixWKZHlPXGZLHddfI4+Mq5CAUJ0BcHmbyv85xpJAVCrJOLL7bMFVTF4zFvo6iVlliuWaDR2K326LqzSl4J3BvSqhHgkR97tc5sDxPkx21W/tge6xpP1r8tYuelDL4UjsUvYE8ffho/vNhye/04b7P9w/oByYvDeGaPs2Ajwbp0lgjBRLuN84goMW4cHbIOmwr/nOQ==', '2TqpTzSi9ilBnovIGGjlsw==', 'b1fae38b-2325-4aa9-b0c3-3a31ef367210', 1, 0, 0
END