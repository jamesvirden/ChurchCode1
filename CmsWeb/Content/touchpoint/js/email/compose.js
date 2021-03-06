﻿$(function () {
    $('#Recipients').select2();
    $('#Recipients').select2("readonly", true);

    var currentDiv = null;

    $.clearFunction = undefined;
    $.addFunction = undefined;

    $.clearTemplateClass = function () {
        if (typeof $.clearFunction != 'undefined') {
            $.clearFunction();
        }
    };

    $.addTemplateClass = function () {
        if (typeof $.addFunction != 'undefined') {
            $.addFunction();
        }
    };

    window.displayEditor = function (div) {
        currentDiv = div;
        $('#editor-modal').modal('show');
    };

    $('#editor-modal').on('shown.bs.modal', function () {
        if ($('#htmleditor').data('fa.editable')) {
            $('#htmleditor').froalaEditable('destroy');
        }

        var extraSmallDevice = $('.device-xs').is(':visible');
        var editorButtons = ['bold', 'italic', 'underline', 'fontSize', 'fontFamily', 'color', 'removeFormat', 'sep', 'formatBlock', 'align', 'insertOrderedList', 'insertUnorderedList', 'outdent', 'indent', 'sep', 'createLink', 'specialLink', 'sep', 'insertImage', 'uploadFile', 'table', 'undo', 'redo', 'html', 'fullscreen'];
        var editorHeight = 400;
        if (extraSmallDevice) {
            editorButtons = ['bold', 'createLink', 'specialLink', 'insertImage', 'html', 'fullscreen'];
            editorHeight = 200;
        }

        $('#htmleditor').froalaEditable({
            inlineMode: false,
            spellcheck: true,
            useFileName: false,
            useClasses: false,
            zIndex: 2501,
            height: editorHeight,
            theme: 'custom',
            buttons: editorButtons,
            imageUploadURL: '/Account/FroalaUpload',
            fileUploadURL: '/Account/FroalaUpload',
            maxFileSize: (1024 * 1024 * 15)
        });
        var html = $(currentDiv).html();
        if (html !== "Click here to edit content") {
            $('#htmleditor').froalaEditable('setHTML', html);
        }
    });

    function adjustIframe() {
        var iFrameID = document.getElementById('email-body');
        if (iFrameID) {
            // here you can make the height, I delete it first, then I make it again
            iFrameID.height = "";
            iFrameID.height = iFrameID.contentWindow.document.body.scrollHeight + 20 + "px";
        }
    }

    $('#editor-modal').on('click', '#save-edit', function () {
        var h = $('#htmleditor').froalaEditable('getHTML');
        $(currentDiv).html(h);
        adjustIframe();
        $('#editor-modal').modal('hide');
    });

    $(".Send").click(function () {
        $.block();
        $('#body').val($('#email-body').contents().find('#tempateBody').html());
        var q = $("#SendEmail").serialize();

        $.post('/Email/QueueEmails', q, function (ret) {
            if (ret && ret.error) {
                $.unblock();
                swal("Error!", ret.error, "error");
            } else {
                if (ret === "timeout") {
                    swal("Session Timeout!", 'Your session timed out. Please copy your email content and start over.', "error");
                    return;
                }
                var taskid = ret.id;
                if (taskid === 0) {
                    $.unblock();
                    swal("Success!", ret.content, "success");
                } else {
                    $('.Send').remove();
                    var intervalid = window.setInterval(function () {
                        $.post('/Email/TaskProgress/' + taskid, null, function (ret) {
                            $.unblock();
                            if (ret && ret.error) {
                                swal("Error!", ret.error, "error");
                            } else {
                                if (ret.title == 'Email has completed.') {
                                    swal(ret.title, ret.message, "success");
                                    window.clearInterval(intervalid);
                                } else {
                                    swal({
                                        title: ret.title,
                                        text: ret.message,
                                        imageUrl: '/Content/touchpoint/img/spinner.gif'
                                    });
                                }
                            }
                        });
                    }, 3000);
                }
            }
        });
    });

    $(".SaveDraft").click(function () {
        if ($(this).attr("saveType") == "0") {
            $('#draft-modal').modal('show');
        } else {
            $.clearTemplateClass();
            $("#body").val($('#email-body').contents().find('#tempateBody').html());
            $("#name").val($("#newName").val());
            $.addTemplateClass();

            $("#SendEmail").attr("action", "/Email/SaveDraft");
            $("#SendEmail").submit();
        }
    });

    $('#draft-modal').on('shown.bs.modal', function () {
        $("#newName").val('').focus();
    });

    $("#SaveDraftButton").click(function () {
        $.clearTemplateClass();
        $("#body").val($('#email-body').contents().find('#tempateBody').html());
        $("#name").val($("#newName").val());
        $.addTemplateClass();

        $("#SendEmail").attr("action", "/Email/SaveDraft");
        $("#SendEmail").submit();
    });

    $(".TestSend").click(function () {
        $.block();

        $.clearTemplateClass();
        $("#body").val($('#email-body').contents().find('#tempateBody').html());
        $.addTemplateClass();

        var q = $("#SendEmail").serialize();

        $.post('/Email/TestEmail', q, function (ret) {
            $.unblock();
            if (ret && ret.error) {
                swal("Error!", ret.error, "error");
            } else {
                if (ret == "timeout") {
                    swal("Session Timeout!", 'Your session timed out. Please copy your email content and start over.', "error");
                    return;
                }
                swal("Success!", ret, "success");
            }
        });
    });

    $('#Subject').focus();
});



