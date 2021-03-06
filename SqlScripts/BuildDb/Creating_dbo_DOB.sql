CREATE FUNCTION dbo.DOB(@m INT, @d INT, @y INT)
RETURNS VARCHAR(20)
AS
BEGIN
	DECLARE @ret VARCHAR(20)
	IF @m IS NOT NULL AND @d IS NOT NULL
		IF @y IS NULL
			SET @ret = CONVERT(VARCHAR, @m) + '/' + CONVERT(VARCHAR, @d)
		ELSE
			SET @ret = CONVERT(VARCHAR, @m) + '/' + CONVERT(VARCHAR, @d) + '/' + CONVERT(VARCHAR, @y)
	ELSE IF @y IS NOT NULL
		IF @m IS NOT NULL AND @d IS NULL
			SET @ret = CONVERT(VARCHAR, @m) + '/1/' + CONVERT(VARCHAR, @y)
		ELSE
			SET @ret = CONVERT(VARCHAR, @y)
	RETURN @ret
END


GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
