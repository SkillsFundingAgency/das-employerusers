CREATE PROCEDURE [dbo].[UpdateUserGovUkIdentifier]
	@email varchar(255),
	@govUkIdentifier varchar(150)
AS
	UPDATE [User] 
	SET [User].[GovUkIdentifier] = @govUkIdentifier
	WHERE [User].[email] = @email
RETURN 0
