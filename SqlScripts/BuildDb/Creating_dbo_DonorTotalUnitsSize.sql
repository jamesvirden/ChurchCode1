CREATE FUNCTION [dbo].[DonorTotalUnitsSize](@t DonorTotalsTable READONLY, @min INT, @max INT)
RETURNS MONEY
AS
BEGIN
	DECLARE @ret MONEY
	SELECT @ret = COUNT(*) FROM @t WHERE tot > (@min - 1) AND tot <= @max
	RETURN @ret
END
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
