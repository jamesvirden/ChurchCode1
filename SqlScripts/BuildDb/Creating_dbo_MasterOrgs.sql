
CREATE VIEW [dbo].[MasterOrgs]
AS

SELECT o.OrganizationId, PickListOrgId, o.OrganizationName FROM Organizations o 
CROSS APPLY ( SELECT value PickListOrgId 
	FROM dbo.SplitInts(o.OrgPickList) ) T 
WHERE o.OrgPickList IS NOT NULL
GO
IF @@ERROR<>0 AND @@TRANCOUNT>0 ROLLBACK TRANSACTION
GO
IF @@TRANCOUNT=0 BEGIN INSERT INTO #tmpErrors (Error) SELECT 1 BEGIN TRANSACTION END
GO
