<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CMSWeb.Models.VolunteersModel>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script src="/Content/js/jquery.pagination.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function() {
            $('#Org').change(RefreshList);
            $('#View').change(RefreshList);
            $('#query').click(function() {
                var q = $('#form').serialize();
                $.navigate("/Volunteers/Query/" + $('#QueryId').val(), q);
            });
            $('#Volunteers > thead a.sortable').click(function(ev) {
                var newsort = $(this).text();
                var oldsort = $("#Sort").val();
                $("#Sort").val(newsort);
                var dir = $("#Dir").val();
                if (oldsort == newsort && dir == 'asc')
                    $("#Dir").val('desc');
                else
                    $("#Dir").val('asc');
                RefreshList();
            });
        });
        (function($) {
            $.fn.fillOptions = function(a, multiple) {
                var options = '';
                if (a)
                    for (var i = 0; i < a.length; i++) {
                    options += '<option value="' + a[i].Value + '"';
                    if (a[i].Selected)
                        options += ' selected=\'selected\''
                    options += '>' + a[i].Text + '</option>';
                }
                return this.each(function() {
                    var s = "<select id='" + this.id + "' name='" + this.id + "'";
                    if (multiple)
                        s += " multiple='multiple'";
                    s += ">" + options + "</select>";
                    $(this).replaceWith(s);
                });
            };
            $.fn.multiSelectRemove = function() {
                $(this).each(function() {
                    $(this).next('.multiSelect').remove();
                    $(this).next('.multiSelectOptions').remove();
                });
                return $(this);
            };
        })(jQuery);
        function RefreshList() {
            var q = $('#form').serialize();
            $.navigate("/Volunteers/Index/" + $('#QueryId').val(), q);
        }
        function GotoPage(pg) {
            var q = $('#form').serialize();
            q = q.appendQuery("Page=" + pg);
            $.navigate("/Volunteers/Index/" + $('#QueryId').val(), q);
        }
        function SetPageSize(sz) {
            var q = $('#form').serialize();
            q = q.appendQuery("PageSize=" + sz);
            $.navigate("/Volunteers/Index/" + $('#QueryId').val(), q);
        }
    </script>
    <%=Html.Hidden("QueryId", Model.QueryId) %>
    <form id="form" method="get" action="/Volunteers/Index">
    <div class="modalPopup">
       Interests: <%=Html.DropDownList("Org", Model.Interests())%>
       Views: <%=Html.DropDownList("View", CMSWeb.Models.VolunteersModel.Views())%>
       <a id="query" href="#">QueryBuilder</a>
    </div>
    <%=Html.Hidden("Sort", Model.Sort) %>
    <%=Html.Hidden("Dir", Model.Dir) %>
    <table id="Volunteers">
        <thead>
        <tr>
            <th></th>
            <th><a href="#" class="sortable">Name</a></th>
            <th>Interests</th>
            <th><a href="#" class="sortable">Application</a></th>
            <th></th>
        </tr>
        </thead>
        <tbody>
        <% foreach (var v in Model.FetchVolunteers())
           { %>
        <tr>
            <td nowrap="nowrap">
                <% if (Model.View != "ns")
                   { %>
                <a href="/Volunteer/PickList2/<%=Model.View %>?pid=<%=v.PeopleId %>"><%=Model.View %> page</a>
                <% } %>
            </td>
            <td nowrap="nowrap">
                <a href='/Person/Index/<%=v.PeopleId%>'><%=v.Name%></a>
            </td>
            <td><%=v.Interests%></td>
            <td>
            <%=v.Application %>
            </td>
        </tr>
        <% } %>
        </tbody>
    </table>
    <% Html.RenderPartial("Pager", Model.pagerModel()); %>
<input type="hidden" id="Count" value='<%=Model.Count%>' />
</form>
</asp:Content>
