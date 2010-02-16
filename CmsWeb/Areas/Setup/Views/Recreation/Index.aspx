<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<CmsData.RecLeague>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/Content/js/jquery.jeditable.js" type="text/javascript"></script>
    <script type="text/javascript">
        //id=elements_id&value=user_edited_content
        $(function() {
            $(".clickEdit").editable("/Setup/Recreation/EditLeague/", {
                indicator: "<img src='/images/loading.gif'>",
                tooltip: "Click to edit...",
                style: 'display: inline',
                width: '200px'
            });
            $("a.delete").click(function(ev) {
                if (confirm("are you sure?"))
                    $.post("/Setup/Recreation/DeleteLeague/" + $(this).attr("id"), null, function(ret) {
                        window.location = "/Setup/Recreation/";
                    });
                return false;
            });
        });
    </script>
    <h2>Recreation Setup</h2>

    <table>
        <tr>
            <th>
                League
            </th>
            <th>
                AgeDate
            </th>
            <th>
                ExtraFee
            </th>
            <th>
                ShirtFee
            </th>
            <th>
                ExpirationDt
            </th>
            <th></th>
            <th></th>
        </tr>

    <% foreach (var item in Model) 
       { %>
        <tr>
            <td>
                <a href="/Setup/Recreation/AgeDivisions/<%=item.DivId %>"><%=item.Division.Name%></a>
            </td>
            <td>
                <span id='a<%=item.DivId %>' 
                    class='clickEdit'><%=item.AgeDate%></span>
            </td>
            <td>
                <span id='e<%=item.DivId %>' 
                    class='clickEdit'><%=item.ExtraFee%></span>
            </td>
            <td>
                <span id='t<%=item.DivId %>' 
                    class='clickEdit'><%=item.ShirtFee%></span>
            </td>
            <td>
                <span id='z<%=item.DivId %>' 
                    class='clickEdit'><%=item.ExpirationDt%></span>
            </td>
            <td><a href="/Display/LeagueContent/<%=item.DivId %>">edit message</a></td>
            <td>
                <a id='x<%=item.DivId %>' href="#" class="delete"><img border="0" src="/images/delete.gif" /></a>
            </td>
        </tr>
    <% } %>

    </table>

    <% using (Html.BeginForm("CreateLeague", "Recreation"))
       { %>
        <p><%=Html.DropDownList("id", ViewData["leagues"] as IEnumerable<SelectListItem>) %>
        <input type="submit" value="Create" />
        <%=Html.ValidationMessage("league") %>
        </p>
    <% } %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
