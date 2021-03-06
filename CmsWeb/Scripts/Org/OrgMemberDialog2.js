﻿$(function () {
    $("#membergroups .ckbox").live("click", function (ev) {
        var f = $(this).closest("form");
        var q = f.serialize() + '&' + $.param({
             'ck': $(this).is(":checked")
        });
        $.post($(this).attr("href"), q);
        return true;
    });
    $("#membergroups .update-smallgroup").live("click", function (ev) {
        ev.preventDefault();
        var href = $(this).attr("href");
        var msg = "This will add or remove everybody to/from this sub-group. Are you sure?";
        bootbox.confirm(msg, function (confirmed) {
            if (confirmed)
                $.post(href);
        });
        return false;
    });
    $('#OrgSearch').live("keydown", function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            $("#orgsearchbtn").click();
        }
    });
    $("a.movemember").live('click', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        var href = $(this).attr("href");
        bootbox.confirm("are you sure?", function (confirmed) {
            if (confirmed) {
                $.post(href, q, function (ret) {
                    f.modal("hide");
                    RebindMemberGrids();
                });
            }
        });
        return false;
    });
});

