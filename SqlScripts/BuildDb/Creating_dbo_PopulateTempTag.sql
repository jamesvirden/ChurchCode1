CREATE PROCEDURE [dbo].[PopulateTempTag] (@id INT, @List VARCHAR(MAX))
AS
BEGIN
    SET NOCOUNT ON;

	INSERT INTO TagPerson(Id, PeopleId) SELECT @id, Value FROM dbo.SplitInts(@List)
	        
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
