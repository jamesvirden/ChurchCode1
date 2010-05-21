﻿/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church 
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UtilityExtensions;
using System.Text;
using CmsData;
using System.Data.Linq.SqlClient;

namespace CMSWeb.Models
{
    public class CouponModel
    {
        public string regid { get; set; }
        public decimal amount { get; set; }
        public string name { get; set; }
        public string couponcode { get; set; }

        public int useridfilter { get; set; }
        public string regidfilter { get; set; }
        public string usedfilter { get; set; }

        public string Registration()
        {
            return OnlineRegs().Single(r => r.Value == regid).Text;
        }

        public IEnumerable<CouponInfo> Coupons()
        {
            var q = from c in DbUtil.Db.Coupons
                    where c.DivOrg == regidfilter || regidfilter == "0" || regidfilter == null
                    where c.UserId == useridfilter || useridfilter == 0
                    select c;
            switch (usedfilter)
            {
                case "Used":
                    q = q.Where(c => c.Used != null);
                    break;
                case "UnUsed":
                    q = q.Where(c => c.Used == null);
                    break;
                case "Expired":
                    q = q.Where(c => SqlMethods.DateDiffHour(c.Created, DateTime.Now) >= 24);
                    break;
            }

            var q2 = from c in q
                     orderby c.Created descending
                     select new CouponInfo
                     {
                         Amount = c.Amount,
                         Canceled = c.Canceled,
                         Code = c.Id,
                         Created = c.Created,
                         OrgDivName = c.OrgId != null ? c.Organization.OrganizationName : c.Division.Name,
                         Used = c.Used,
                         PeopleId = c.PeopleId,
                         Name = c.Name,
                         Person = c.Person.Name,
                         UserId = c.UserId,
                         UserName = c.User.Name,
                         RegAmt = c.RegAmount
                     };
            return q2.Take(200);
        }
        public List<SelectListItem> OnlineRegs()
        {
            var orgregtypes = new int[] { 1, 2 };
            var divregtypes = new int[] { 3, 4 };
            var q = from o in DbUtil.Db.Organizations
                    where orgregtypes.Contains(o.RegistrationTypeId.Value)
                    where o.ClassFilled != true
                    where (o.RegistrationClosed ?? false) == false
                    where o.Fee > 0
                    select new SelectListItem
                    {
                        Text = o.Division.Name + ": " + o.OrganizationName,
                        Value = "org." + o.OrganizationId
                    };
            var q2 = from o in DbUtil.Db.Organizations
                     where divregtypes.Contains(o.RegistrationTypeId.Value)
                     where o.ClassFilled != true
                     where (o.RegistrationClosed ?? false) == false
                     where o.Fee > 0
                     group o.Division by o.DivisionId into g
                     from d in g
                     select new SelectListItem
                     {
                         Text = d.Name,
                         Value = "div." + d.Id
                     };
            var list = q.Union(q2).OrderBy(n => n.Text).ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public List<SelectListItem> Users()
        {
            var q = from c in DbUtil.Db.Coupons
                    where c.UserId != null
                    group c by c.UserId into g
                    select new SelectListItem
                    {
                        Value = g.Key.ToString(),
                        Text = g.First().User.Name,
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(not specified)", Value = "0" });
            return list;
        }
        public List<SelectListItem> CouponStatus()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "(not specified)" },
                new SelectListItem { Text = "Used" },
                new SelectListItem { Text = "UnUsed" },
                new SelectListItem { Text = "Expired" },
            };
        }
        public Coupon CreateCoupon()
        {
            var existing = true;
            string code;
            do
            {
                code = Util.RandomPassword(12);
                var q = from cp in DbUtil.Db.Coupons
                        where cp.Id == code
                        where cp.Used == null && cp.Canceled == null
                        select cp;
                existing = q.SingleOrDefault() != null;
            }
            while (existing);

            var c = new Coupon
            {
                Id = code,
                Created = DateTime.Now,
                Amount = amount,
                Name = name,
                UserId = Util.UserId,
            };
            SetDivOrgIds(c);
            DbUtil.Db.Coupons.InsertOnSubmit(c);
            DbUtil.Db.SubmitChanges();
            couponcode = c.Id.Insert(8, " ").Insert(4, " ");

            return c;
        }
        private void SetDivOrgIds(Coupon c)
        {
            if (regid.HasValue())
            {
                var a = regid.Split('.');
                switch (a[0])
                {
                    case "org":
                        c.OrgId = a[1].ToInt();
                        break;
                    case "div":
                        c.DivId = a[1].ToInt();
                        break;
                }
            }
        }
        public class CouponInfo
        {
            public string Code { get; set; }
            public string Coupon
            {
                get { return Code.Insert(8, " ").Insert(4, " "); }
            }
            public string OrgDivName { get; set; }
            public DateTime Created { get; set; }
            public DateTime? Used { get; set; }
            public DateTime? Canceled { get; set; }
            public decimal? Amount { get; set; }
            public decimal? RegAmt { get; set; }
            public int? PeopleId { get; set; }
            public string Name { get; set; }
            public string Person { get; set; }
            public int? UserId { get; set; }
            public string UserName { get; set; }
        }
    }
}