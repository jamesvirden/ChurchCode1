﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CmsWeb.Areas.Reports.ViewModels
{
    [Serializable]
    public class CustomReportViewModel
    {
        public bool CustomReportSuccessfullySaved { get; set; }

        public int? OrgId { get; set; }

        public string OriginalReportName { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z0-9 ]+$", ErrorMessage = "The report name can only contain alphanumeric characters or spaces. (a-z, 0-9)")]
        public string ReportName { get; set; }

        public List<CustomReportColumn> Columns { get; set; }

        public bool RestrictToThisOrg { get; set; }
        public Guid QueryId { get; set; }
        public string OrgName { get; set; }

        public CustomReportViewModel() {} // for model binding purposes

        public CustomReportViewModel(int? orgId, Guid queryId, string orgName, IEnumerable<CustomReportColumn> standardColumns, string reportName) 
            : this(orgId, queryId, orgName, standardColumns)
        {
            ReportName = reportName;
            OriginalReportName = reportName;
        }

        public CustomReportViewModel(int? orgId, Guid queryId, string orgName, IEnumerable<CustomReportColumn> standardColumns)
        {
            OrgId = orgId;
            QueryId = queryId;
            OrgName = orgName;
            Columns = new List<CustomReportColumn>();
            Columns.AddRange(standardColumns);
        }

        public void SetSelectedColumns(IEnumerable<CustomReportColumn> columns)
        {
            var columnsAsList = columns.ToList();

            foreach (var column in Columns)
            {
                if (column.IsStatusFlag)
                {
                    if (columnsAsList.Select(c => c.Description).Contains(column.Description))
                        column.IsSelected = true;
                }
                else if (column.IsExtraValue)
                {
                    if (columnsAsList.Select(c => c.Field).Contains(column.Field))
                    {
                        column.IsSelected = true;
                        column.IsDisabled = false;
                    }
                }
                else if (column.IsSmallGroup)
                {
                    if (columnsAsList.Select(c => c.SmallGroup).Contains(column.SmallGroup))
                        column.IsSelected = true;
                }
                else
                {
                    if (columnsAsList.Select(c => c.Name).Contains(column.Name))
                        column.IsSelected = true;
                }
            }
        }
    }

    [Serializable]
    public class CustomReportColumn
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Field { get; set; }
        public string Flag { get; set; }
        public string OrgId { get; set; }
        public bool IsDisabled { get; set; }
        public string SmallGroup { get; set; }

        public bool IsSelected { get; set; }

        public bool IsStatusFlag { get { return !string.IsNullOrEmpty(Flag); } }
        public bool IsExtraValue { get { return !string.IsNullOrEmpty(Field); } }
        public bool IsSmallGroup { get { return !string.IsNullOrEmpty(SmallGroup); } }
    }
}