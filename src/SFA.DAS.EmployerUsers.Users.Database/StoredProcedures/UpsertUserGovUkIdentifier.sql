CREATE PROCEDURE [dbo].[UpsertUserGovUkIdentifier]
	@email varchar(255),
	@govUkIdentifier varchar(150),
    @firstName varchar(50),
    @lastName varchar(50)
AS
    IF NOT EXISTS (select top 1 1 From [User] where email = @email)
        BEGIN
            INSERT INTO [User] (ID, Email, FirstName, LastName, GovUkIdentifier)
            VALUES(NEWID(), @Email, @firstName, @lastName, @govUkIdentifier )
        END
    ELSE
        BEGIN
            UPDATE [User]
            SET
                [User].[GovUkIdentifier] = @govUkIdentifier,
                [User].[FirstName] = @firstName,
                [User].[LastName] = @lastName
            WHERE [User].[email] = @email
        END
RETURN 0