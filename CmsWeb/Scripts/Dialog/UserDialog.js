﻿$(function () {
    $(".submitbutton").button();
    $("body").on('click', '.save', function (ev) {
        ev.preventDefault();
        var f = $(this).closest('form');
        var q = f.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            if (ret != "ok") 
                $.block(ret);
            else
                self.parent.RebindUserInfoGrid();
        });
        return false;
    });
    $("body").on('click', '#deluser', function (ev) {
        ev.preventDefault();
        if (!confirm("are you sure you want to delete?"))
            return false;
        var f = $(this).closest('form');
        var q = f.serialize();
        $.post($(this).attr('href'), q, function (ret) {
            self.parent.RebindUserInfoGrid();
        });
        return false;
    });
    $(".tip").tooltip({ showBody: "|", showURL: false });
});

