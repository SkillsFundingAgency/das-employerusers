CREATE PROCEDURE [dbo].[SearchUsers]
	@criteria NVARCHAR(255),
	@pageSize int,
	@offset int,
	@totalRecords INT OUTPUT
AS

	SET @criteria = '%' + UPPER(@criteria) + '%'

	SELECT
		Id,
		FirstName,
		LastName,
		Email,
		PasswordProfileId,
		IsActive,
		FailedLoginAttempts,
		IsLocked
	INTO #resultSet
	FROM dbo.[User]
	WHERE UPPER(FirstName) LIKE @criteria OR UPPER(LastName) LIKE @criteria OR UPPER(Email) LIKE @criteria

	SELECT @totalRecords = COUNT(*) FROM #resultSet
	
	SELECT * FROM #resultSet
	ORDER BY ID
	OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY

GO
