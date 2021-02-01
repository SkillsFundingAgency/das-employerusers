CREATE PROCEDURE [dbo].[UpdateUserSuspension]
	@Id varchar(50),
	@state bit
AS
	UPDATE [User] SET [User].[IsSuspended] = @state WHERE [User].[Id] = @Id
RETURN 0
