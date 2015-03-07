﻿using System.Web.Mvc;
using CmsWeb.Areas.Org2.Dialog.Models;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Controllers
{
    [RouteArea("Organization", AreaPrefix="RepairTransactions"), Route("{action}/{id?}")]
    public class RepairTransactionsController : CmsStaffController
    {
        [HttpPost, Route("~/RepairTransactions/{id:int}")]
        public ActionResult Index(int id)
        {
            var model = new RepairTransactions(id);
            model.RemoveExistingLop(DbUtil.Db, id, RepairTransactions.Op);
            return View(model);
        }

        [HttpPost]
        public ActionResult Process(RepairTransactions model)
        {
            model.UpdateLongRunningOp(DbUtil.Db, RepairTransactions.Op);

            if (!model.Started.HasValue)
            { 
                DbUtil.LogActivity("Add to org from tag for {0}".Fmt(Session["ActiveOrganization"]));
                model.Process(DbUtil.Db);
            }
			return View(model);
		}
    }
}