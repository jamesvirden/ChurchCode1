﻿using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;
using System.Xml.XPath;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class ReportsMenuModel
    {
        private static XDocument CustomReportsMenu
        {
            get
            {

                var db = DbUtil.Db;
#if DEBUG2
                var s = Resource1.ReportsMenuCustom;
#else
                var s = HttpRuntime.Cache[DbUtil.Db.Host + "CustomReportsMenu"] as string;
                if (s == null)
                {
                    s = db.ContentText("CustomReportsMenu", Resource1.ReportsMenuCustom);
                    HttpRuntime.Cache.Insert(db.Host + "CustomReportsMenu", s, null,
                        DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
                }
                if (!s.HasValue())
                    return null;
#endif
                var xdoc = XDocument.Parse(s);
                return xdoc;
            }
        }

        private static XDocument ReportsMenu
        {
            get { return XDocument.Parse(Resource1.ReportsMenu); }
        }

        public static string Items
        {
            get
            {
                var xdoc = ReportsMenu;
                if (xdoc == null || xdoc.Root == null)
                    return null;
                return ReportItems(xdoc, "/ReportsMenu");
            }
        }

        public static string CustomItems1
        {
            get
            {
                var xdoc = CustomReportsMenu;
                if (xdoc?.Root == null)
                    return null;
                return ReportItems(xdoc, "/ReportsMenu/Column1");
            }
        }
        public static string CustomItems2
        {
            get
            {
                var xdoc = CustomReportsMenu;
                if (xdoc?.Root == null)
                    return null;
                return ReportItems(xdoc, "/ReportsMenu/Column2");
            }
        }

        private static string ReportItems(XDocument xdoc, string path)
        {
            var sb = new StringBuilder();
            foreach (var e in xdoc.XPathSelectElements(path).Elements())
            {
                var roles = ((string)e.Attribute("roles"))?.Split(',');
                if (roles != null)
                    if (!roles.Any(rr => DbUtil.Db.CurrentUser.Roles.Contains(rr)))
                        continue;

                var tb = new TagBuilder("li");
                switch (e.Name.LocalName)
                {
                    case "Report":
                        var a = new TagBuilder("a");
                        a.MergeAttribute("href", e.Attribute("link").Value);
                        var t = e.Attribute("target");
                        if (t != null)
                            a.MergeAttribute("target", t.Value);
                        a.SetInnerText(e.Value);
                        tb.InnerHtml = a.ToString();
                        break;
                    case "Header":
                        tb.AddCssClass("dropdown-header");
                        tb.SetInnerText(e.Value);
                        break;
                    case "Space":
                        tb.AddCssClass("divider");
                        break;
                }
                sb.AppendLine(tb.ToString());
            }
            return sb.ToString();
        }
    }
}