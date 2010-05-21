﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;
using System.Web.Mvc;

namespace CMSWeb.Models.OrganizationPage
{
    public class MemberModel
    {
        public enum GroupSelect
        {
            Active,
            Inactive,
            Pending,
        }
        public int OrganizationId { get; set; }
        private int GroupId;
        private GroupSelect Select;

        public PagerModel2 Pager { get; set; }
        public MemberModel(int id, int groupid, GroupSelect select)
        {
            OrganizationId = id;
            GroupId = groupid;
            Select = select;
            Pager = new PagerModel2(Count);
            Pager.Direction = "asc";
            Pager.Sort = "Name";
        }
        public IEnumerable<SelectListItem> SmallGroups()
        {
            var q = from mt in DbUtil.Db.MemberTags
                    where mt.OrgId == OrganizationId
                    orderby mt.Name
                    select new SelectListItem
                    {
                        Text = mt.Name,
                        Value = mt.Id.ToString()
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Value = "-1", Text = "(not assigned)" });
            list.Insert(0, new SelectListItem { Value = "0", Text = "(not specified)" });
            return list;
        }
        private IQueryable<OrganizationMember> _members;
        private IQueryable<OrganizationMember> FetchMembers()
        {
            int inactive = (int)OrganizationMember.MemberTypeCode.InActive;
            bool Active = true;
            bool Pending = false;
            switch (Select)
            {
                case GroupSelect.Inactive:
                    Active = false;
                    Pending = false;
                    break;
                case GroupSelect.Pending:
                    Active = true;
                    Pending = true;
                    break;
                //case GroupSelect.Previous:
                //    return PrevOrgMembers(OrganizationId, sortExpression, maximumRows, startRowIndex);
            }

            if (_members == null)
                _members = from om in DbUtil.Db.OrganizationMembers
                           where om.OrganizationId == OrganizationId
                           where om.OrgMemMemTags.Any(mt => mt.MemberTagId == GroupId) || GroupId <= 0
                           where om.OrgMemMemTags.Count() == 0 || GroupId != -1
                           where (Active && om.MemberTypeId != inactive)
                               || (!Active && om.MemberTypeId == inactive)
                           where (Pending && om.Pending == true)
                               || (!Pending && (om.Pending ?? false) == false)
                           select om;
            return _members;
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchMembers().Count();
            return _count.Value;
        }
        public IEnumerable<PersonMemberInfo> Members()
        {
            var q0 = ApplySort();
            q0 = q0.Skip(Pager.StartRow).Take(Pager.PageSize);
            var tagownerid = Util.CurrentTagOwnerId;
            var q = from om in q0
                    let p = om.Person
                    select new PersonMemberInfo
                    {
                        PeopleId = p.PeopleId,
                        Name = p.Name,
                        Name2 = p.Name2,
                        BirthDate = Util.FormatBirthday(
                            p.BirthYear,
                            p.BirthMonth,
                            p.BirthDay),
                        Address = p.PrimaryAddress,
                        Address2 = p.PrimaryAddress2,
                        CityStateZip = Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip),
                        EmailAddress = p.EmailAddress,
                        PhonePref = p.PhonePrefId,
                        HomePhone = p.HomePhone,
                        CellPhone = p.CellPhone,
                        WorkPhone = p.WorkPhone,
                        MemberStatus = p.MemberStatus.Description,
                        Email = p.EmailAddress,
                        BFTeacher = p.BFClass.LeaderName,
                        BFTeacherId = p.BFClass.LeaderId,
                        Age = p.Age.ToString(),
                        MemberTypeCode = om.MemberType.Code,
                        MemberType = om.MemberType.Description,
                        MemberTypeId = om.MemberTypeId,
                        InactiveDate = om.InactiveDate,
                        AttendPct = om.AttendPct,
                        LastAttended = om.LastAttended,
                        HasTag = p.Tags.Any(t => t.Tag.Name == Util.CurrentTagName && t.Tag.PeopleId == tagownerid),
                        //FromTab = fromtab,
                        Joined = om.EnrollmentDate,
                    };
            return q;
        }
        public IQueryable<OrganizationMember> ApplySort()
        {
            var q = FetchMembers();
            if (Pager.Direction == "asc")
                switch (Pager.Sort)
                {
                    case "Name":
                        q = from om in q
                            let p = om.Person
                            orderby p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "Church":
                        q = from om in q
                            let p = om.Person
                            orderby p.MemberStatus.Code,
                            p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "Member":
                        q = from om in q
                            let p = om.Person
                            orderby om.MemberType.Code,
                            p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "Primary Address":
                        q = from om in q
                            let p = om.Person
                            orderby p.Family.StateCode,
                            p.Family.CityName,
                            p.Family.AddressLineOne,
                            p.PeopleId
                            select om;
                        break;
                    case "BFTeacher":
                        q = from om in q
                            let p = om.Person
                            orderby p.BFClass.LeaderName,
                            p.Name2,
                            p.PeopleId
                            select om;
                        break;
                    case "% Att.":
                        q = from om in q
                            orderby om.AttendPct
                            select om;
                        break;
                    case "Age":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthYear, p.BirthMonth, p.BirthDay
                            select om;
                        break;
                    case "Bday":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthMonth, p.BirthDay,
                            p.Name2
                            select om;
                        break;
                    case "Last Att.":
                        q = from om in q
                            let p = om.Person
                            orderby om.LastAttended, p.LastName, p.FirstName
                            select om;
                        break;
                }
            else
                switch (Pager.Sort)
                {
                    case "Church":
                        q = from om in q
                            let p = om.Person
                            orderby p.MemberStatus.Code descending,
                            p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "Member":
                        q = from om in q
                            let p = om.Person
                            orderby om.MemberType.Code descending,
                            p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "Address":
                        q = from om in q
                            let p = om.Person
                            orderby p.Family.StateCode descending,
                                   p.Family.CityName descending,
                                   p.Family.AddressLineOne descending,
                                   p.PeopleId descending
                            select om;
                        break;
                    case "BFTeacher":
                        q = from om in q
                            let p = om.Person
                            orderby p.BFClass.LeaderName descending,
                            p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "% Att.":
                        q = from om in q
                            orderby om.AttendPct descending
                            select om;
                        break;
                    case "Name":
                        q = from om in q
                            let p = om.Person
                            orderby p.Name2,
                            p.PeopleId descending
                            select om;
                        break;
                    case "Bday":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthMonth descending, p.BirthDay descending,
                            p.Name2 descending
                            select om;
                        break;
                    case "Last Att.":
                        q = from om in q
                            let p = om.Person
                            orderby om.LastAttended descending, p.LastName descending, p.FirstName descending
                            select om;
                        break;
                    case "Age":
                        q = from om in q
                            let p = om.Person
                            orderby p.BirthYear descending, p.BirthMonth descending, p.BirthDay descending
                            select om;
                        break;
                }
            return q;
        }
    }
}