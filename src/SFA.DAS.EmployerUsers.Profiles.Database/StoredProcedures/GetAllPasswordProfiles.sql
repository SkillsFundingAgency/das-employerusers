CREATE PROCEDURE GetAllPasswordProfiles
AS
	SELECT
		Id,
		[Key],
		WorkFactor,
		SaltLength,
		StorageLength
	FROM PasswordProfile
GO