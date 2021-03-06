﻿!function (a) {
    a.Editable.commands = a.extend(a.Editable.commands, {
        specialLink: {
            title: 'Special Link',
            icon: 'fa fa-link tp-gold',
            callback: function() {
                this.insertSpecialLink();
                this.$special_link_wrapper.find('select').first().focus();
            },
            undo: true
        }
    });

    a.Editable.DEFAULTS = a.extend(a.Editable.DEFAULTS, {});

    function getPropertiesFromLink(link) {
        var $link = $(link);

        var smallGroupVal = '';
        if ($link.attr('rel') !== 'nofollow') {
            smallGroupVal = $link.attr('rel');
        }

        return {
            linkType: $link.attr('href').replace('https://', ''),
            orgId: $link.attr('lang'),
            message: $link.attr('title'),
            smallGroup: smallGroupVal,
            confirmation: $link.attr('dir'),
            openInNewTab: $link.attr('target') === '_blank'
        };
    }

    function setSpecialLinkValues(pluginInstance, props) {
        var id = pluginInstance._id;
        var wrapper = pluginInstance.$special_link_wrapper;

        wrapper.find('#f-special-link-type-' + id).val(props.linkType);
        wrapper.find('#f-special-link-orgid-' + id).val(props.orgId);
        wrapper.find('#f-special-link-message-' + id).val(props.message);
        wrapper.find('#f-special-link-small-group-' + id).val(props.smallGroup);
        wrapper.find('#f-special-link-confirmation-' + id).val(props.confirmation);
        wrapper.find('#f-special-link-new-tab-' + id).prop('checked', props.openInNewTab);
    }

    a.Editable.prototype.showSpecialLinkWrapper = function() {
        var links = this.getSelectionLinks();

        if (links.length > 0) {
            var link = links[0];
            var props = getPropertiesFromLink(link);

            setSpecialLinkValues(this, props);
        }
        else {
            var defaultValues = {
                linkType: '',
                orgId: '',
                message: '',
                smallGroup: '',
                confirmation: '',
                openInNewTab: false
            };

            setSpecialLinkValues(this, defaultValues);
        }

        this.$special_link_wrapper && this.$special_link_wrapper.show();
    };

    a.Editable.prototype.hideSpecialLinkWrapper = function() {
        this.$special_link_wrapper && (this.$special_link_wrapper.hide(), this.$special_link_wrapper.find('input').blur());
    };

    a.Editable.prototype.showSpecialLink = function() {
        this.hidePopups();
        this.showSpecialLinkWrapper();
    };

    a.Editable.prototype.insertSpecialLink = function() {
        this.saveSelectionByMarkers();
        this.showSpecialLink();
        this.options.inlineMode || this.positionPopup('specialLink');
    };

    a.Editable.prototype.specialLinkHTML = function() {
        var b = '<div class="froala-popup froala-link-popup" style="display: none;"><h4><span data-text="true">Create Special Link</span><i title="Cancel" class="fa fa-times" id="f-special-link-close-' + this._id + '"></i></h4>';
        return b += '<div class="f-popup-line">' +
            '<div><strong>Type</strong></div>' +
            '<select id="f-special-link-type-' + this._id + '" style="width: 250px; height: 25px; border: solid 1px #cccccc;">' +
            '<option value="">&lt;not set&gt;</option>' +
            '<option value="registerlink">Register Link</option>' +
            '<option value="registerlink2">Register Link 2</option>' +
            '<option value="regretslink">Regrets Link</option>' +
            '<option value="rsvplink">RSVP Link</option>' +
            '<option value="sendlink">Send Link</option>' +
            '<option value="sendlink2">Send Link 2</option>' +
            '<option value="supportlink">Support Link</option>' +
            '<option value="votelink">Vote Link</option>' +
            '<option value="masterlink">Master Link (NOT SUPPORTED YET)</option>' +
            '</select>' +
            '</div>' +
            '<div class="f-popup-line">' +
            '<div><strong>OrgId / Meeting Id</strong></div>' +
            '<input type="text" class="f-lu" id="f-special-link-orgid-' + this._id + '">' +
            '</div>' +
            '<div class="f-popup-line">' +
            '<div><strong>Message</strong></div>' +
            '<input type="text" class="f-lu" id="f-special-link-message-' + this._id + '"></div>' +
            '<div class="f-popup-line">' +
            '<div><strong>Small Group</strong></div>' +
            '<input type="text" class="f-lu" id="f-special-link-small-group-' + this._id + '"></div>' +
            '<div class="f-popup-line">' +
            '<div><strong>Confirmation</strong></div>' +
            '<select id="f-special-link-confirmation-' + this._id + '" style="width: 250px; height: 25px; border: solid 1px #cccccc;">' +
            '<option value="">&lt;not set&gt;</option>' +
            '<option value="ltr">Yes, send confirmation</option>' +
            '<option value="rtl">No, do not send confirmation</option>' +
            '</select>' +
            '</div>' +
            '<div class="f-popup-line">' +
            '<input type="checkbox" id="f-special-link-new-tab-' + this._id + '"> ' +
            '<label data-text="true" for="f-special-link-new-tab-' + this._id + '">Open in new tab</label>' +
            '<button id="f-special-link-submit-' + this._id + '" type="button" data-text="true" class="fr-p-bttn f-ok f-submit">OK</button>' +
            '</div>' +
            '</div>';
    };

    a.Editable.prototype.createSpecialLink = function() {
        this.$special_link_wrapper = a(this.specialLinkHTML()), this.$popup_editor.append(this.$special_link_wrapper);

        this.addListener('hidePopups', a.proxy(function () {
            this.hideSpecialLinkWrapper();
        }), this);

        // Stop event propagation in link.
        this.$special_link_wrapper.on('mouseup touchend', $.proxy(function (e) {
            if (!this.isResizing()) {
                e.stopPropagation();
            }
        }, this));

        this.$special_link_wrapper.on('click', function (e) {
            e.stopPropagation();
        });

        this.$special_link_wrapper.on('click', '*', function (e) {
            e.stopPropagation();
        });

        this.$special_link_wrapper.on(this.mouseup, '#f-special-link-submit-' + this._id, a.proxy(function(a) {
            a.stopPropagation();
            a.preventDefault();
            if (this.buildSpecialLink()) {
                this.$bttn_wrapper.show();
                this.hideSpecialLinkWrapper();
                this.restoreSelection();
                this.focus();
                this.hide();
            }
        }, this));

        this.$special_link_wrapper.on(this.mouseup, '#f-special-link-close-' + this._id, a.proxy(function(a) {
            a.stopPropagation();
            a.preventDefault();
            this.$bttn_wrapper.show();
            this.hideSpecialLinkWrapper();
            this.restoreSelection();
            this.focus();
            this.hide();
        }, this));
    };

    a.Editable.prototype.buildSpecialLink = function() {
        var b = this;
        var linkType = b.$special_link_wrapper.find('#f-special-link-type-' + b._id);
        var orgId = b.$special_link_wrapper.find('#f-special-link-orgid-' + b._id);
        var message = b.$special_link_wrapper.find('#f-special-link-message-' + b._id);
        var smallGroup = b.$special_link_wrapper.find('#f-special-link-small-group-' + b._id);
        var confirmation = b.$special_link_wrapper.find('#f-special-link-confirmation-' + b._id);
        var newTab = b.$special_link_wrapper.find('#f-special-link-new-tab-' + b._id);

        if (linkType.val().length > 0) {
            b.restoreSelectionByMarkers();
            b.writeLink('https://' + linkType.val(), null, null, newTab.is(':checked'), true);

            var link = b.getSelectionElements();
            if (confirmation.val().length > 0) {
                $('a', link).attr('dir', confirmation.val());
            }
            if (orgId.val().length > 0) {
                $('a', link).attr('lang', orgId.val());
            }
            if (message.val().length > 0) {
                $('a', link).attr('title', message.val());
            }
            if (smallGroup.val().length > 0) {
                $('a', link).attr('rel', smallGroup.val());
            }

            linkType.val('');
            orgId.val('');
            message.val('');
            smallGroup.val('');
            confirmation.val('');
            newTab.prop('checked', false);

            return true;
        }
        else {
            return false;
        }
    };

    a.Editable.initializers.push(a.Editable.prototype.createSpecialLink);
}(jQuery);
