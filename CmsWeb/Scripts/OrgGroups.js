﻿$(function() {
    $.fmtTable = function() {
        $("table.grid td.tip").tooltip({ showBody: "|" });
        $('table.grid > tbody > tr:even').addClass('alt');
    }
    $.fmtTable();
    $.loadTable = function() {
        $.blockUI();
        $.getTable($('#groupsform'));
        $.unblockUI();
    }
    $('#Filter').live("click", function(ev) {
        ev.preventDefault();
        $.loadTable();
    });
    $("#groupsform").delegate("#memtype", "change", $.loadTable);

    $("#ingroup, #notgroup").keypress(function(ev) {
        if (ev.keyCode == '13') {
            ev.preventDefault();
            $.loadTable();
        }
    });

    $("#groupsform").delegate("#groupid", "change", $.loadTable);
    $.getTable = function(f) {
        var q = f.serialize();
        $.post("/OrgGroups/Filter", q, function(ret) {
            $('table.grid > tbody').html(ret).ready($.fmtTable);
        });
        return false;
    }
    $(".datepicker").datepicker({
        dateFormat: 'm/d/yy',
        changeMonth: true,
        changeYear: true
    });

    $("#SelectAll").click(function() {
        $("input[name='list']").attr('checked', $(this).attr('checked'));
    });
    $("a.display").live('click', function(ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        $.post(this.href, q, function(ret) {
            $(f).html(ret).ready(function() {
                return false;
            });
        });
        return false;
    });
    $("a.groupmanager").live('click', function(ev) {
        ev.preventDefault();
        if (confirm("are you sure?")) {
            var f = $(this).closest('form');
            var q = f.serialize();
            $.blockUI();
            $.post($(this).attr('href'), q, function(ret) {
                if (ret) {
                    f.html(ret).ready(function() {
                        if ($('#newgid').val())
                            $('#groupid').val($('#newgid').val());
                        $('#GroupName').val('');
                        $.fmtTable();
                    });
                }
                $.unblockUI();
            });
        }
    });
    $("form").submit(function(ev) {
        ev.preventDefault();
        return false;
    });
    $.performAction = function(action) {
        if ($('#groupid').val() <= 0) {
            alert("select target group first");
            return false;
        }
        $.blockUI();
        var q = $('form').serialize();
        $.post(action, q, function(ret) {
            $("table.grid > tbody").html(ret).ready($.fmtTable);
            $.unblockUI();
        });
        return false;
    };
    $('#AssignSelectedToTargetGroup').live('click', function(ev) {
        $.performAction("/OrgGroups/AssignSelectedToTargetGroup");
    });
    $('#RemoveSelectedFromTargetGroup').live('click', function(ev) {
        $.performAction("/OrgGroups/RemoveSelectedFromTargetGroup");
    }); 
});
