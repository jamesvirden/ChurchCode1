<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<CMSWeb.Areas.Setup.Controllers.LookupController.Row>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="/Content/js/jquery.jeditable.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(function() {
            $(".clickEdit").editable("/Setup/Lookup/Edit/", {
                indicator: "<img src='/images/loading.gif'>",
                tooltip: "Click to edit...",
                style: 'display: inline',
                width: '200px'
            });
            $("a.delete").click(function(ev) {
                if (confirm("are you sure?"))
                    $.post("/Setup/Lookup/Delete/" + $(this).attr("id"), { type: $('#type').val() }, function(ret) {
                        window.location = "/Setup/Lookup/Index/" + $('#type').val();
                    });
            });
        });
    </script>
   <h2><%=ViewData["type"] %></h2>

    <table>
        <tr>
            <th>
                Id
            </th>
            <th>
                Code
            </th>
            <th>
                Description
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) 
       { %>
        <tr>
            <td><%=item.Id %></td>
            <td>
                <span id='c<%=item.Id %>.<%=ViewData["type"] %>' class='clickEdit'><%=item.Code%></span>
            </td>
            <td>
                <span id='t<%=item.Id %>.<%=ViewData["type"] %>' class='clickEdit'><%=item.Description%></span>
            </td>
            <td>
                <a id='d<%= item.Id %>' href="#" class="delete"><img border="0" src="/images/delete.gif" /></a>
            </td>
        </tr>
    <% } %>

    </table>

    <% using (Html.BeginForm("Create", "Lookup"))
       { %>
    <%=Html.Hidden("type", ViewData["type"]) %>
    <p>
        New Id: <input type="text" name="pk" />
        <input type="submit" value="Create" />
    </p>
    <% } %>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
