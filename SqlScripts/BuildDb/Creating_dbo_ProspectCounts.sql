CREATE VIEW [dbo].[ProspectCounts] AS
SELECT OrganizationId, COUNT(*) prospectcount
FROM dbo.OrganizationMembers
WHERE MemberTypeId = 311
GROUP BY OrganizationId
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
