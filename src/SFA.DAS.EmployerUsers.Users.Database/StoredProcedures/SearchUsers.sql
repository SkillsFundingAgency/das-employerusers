CREATE PROCEDURE [dbo].[SearchUsers]
	@criteria NVARCHAR(255),
	@pageSize int,
	@offset int,
	@totalRecords INT OUTPUT
AS

	SET @criteria = '%' + @criteria + '%'

	SELECT
		Id,
		FirstName,
		LastName,
		Email,
		PasswordProfileId,
		IsActive,
		FailedLoginAttempts,
		IsLocked,
		IsSuspended
	INTO #resultSet
	FROM dbo.[User]
	WHERE FirstName LIKE @criteria 
	OR LastName LIKE @criteria 
	OR FirstName + ' ' + LastName LIKE @criteria
	OR Email LIKE @criteria

	SELECT @totalRecords = COUNT(1) FROM #resultSet
	
	SELECT * FROM #resultSet
	ORDER BY Id
	OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY

GO
