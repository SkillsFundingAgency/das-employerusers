CREATE PROCEDURE [dbo].[UserCount]
AS
	SELECT
		count(*)
	FROM dbo.[User]
GO
