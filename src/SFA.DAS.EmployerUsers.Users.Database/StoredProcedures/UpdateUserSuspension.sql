CREATE PROCEDURE [dbo].[UpdateUserSuspension]
	@Id varchar(50),
	@state bit,
	@suspendedDate datetime = NULL
AS
	UPDATE [User] 
	SET [User].[IsSuspended] = @state,
	[User].[LastSuspendedDate] = COALESCE(@suspendedDate, [User].LastSuspendedDate)
	WHERE [User].[Id] = @Id
RETURN 0
