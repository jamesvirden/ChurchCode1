﻿<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<CmsWeb.Areas.Main.Controllers.TagDuplicatesStatus>" %>
<html>
<head>
    <title>Tag Duplicates Progress</title>
    <meta http-equiv="refresh" content="5" />
</head>
<body>
    <h2>Tag Duplicates Progress</h2>
    <% if (Model != null)
       { %>
    <table>
    <tr><td>Processed</td><td><%=Model.processed %></td></tr>
    <tr><td>Found</td><td><%=Model.found %></td></tr>
    <tr><td>Speed</td><td><%=Model.speed %></td></tr>
    <tr><td>Time</td><td><%=Model.time %></td></tr>
    </table>
    <% } %>
</body>
</html>