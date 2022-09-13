/**
 * A Javascript module to loadeding/refreshing options of a select2 list box using ajax based on selection of another select2 list box.
 * 
 * @url : https://gist.github.com/ajaxray/187e7c9a00666a7ffff52a8a69b8bf31
 * @auther : Anis Uddin Ahmad <anis.programmer@gmail.com>
 * 
 * Live demo - https://codepen.io/ajaxray/full/oBPbQe/
 * w: http://ajaxray.com | t: @ajaxray
 */
var Select2Cascade = (function (window, $) {

    function Select2Cascade(parent, child, url, select2Options, selectValue, selectText, placeholderText = '') {
        var afterActions = [];
        var options = select2Options || { width: '100%', theme: 'bootstrap'};

        // Register functions to be called after cascading data loading done
        this.then = function (callback) {
            afterActions.push(callback);
            return this;
        };

        parent.select2(select2Options).on("change", function (e) {
            child.prop("disabled", true);
            var val = child.val();
            if (!(val != null && val.length > 0)) {
                val = child.attr("data-value");
            }
            console.log(val);
            var _this = this;
            $.ajax({
                type: 'GET',
                url: url + "?parentId=" + $(this).val()+"&isAll=1",
                dataType: 'json',
                data: {},
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.msg == "successful") {
                        var newOptions = "";
                        if (placeholderText.length > 0) {
                             newOptions = '<option value="">'+placeholderText+'</option>';
                        } else {
                             newOptions = '<option value="">Tất cả</option>';
                        }
                        for (var i = 0; i < response.content.length; i++) {
                            newOptions += '<option value="' + response.content[i][selectValue] + '">' + response.content[i][selectText] + '</option>';
                        }
                        child.select2('destroy').html(newOptions).prop("disabled", false)
                            .select2(options);
                        if (val != null && val.length > 0) {
                            child.val(val).trigger('change');
                        }
                        afterActions.forEach(function (callback) {
                            callback(parent, child, response.content);
                        });
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                }
            });
        });

        if (parent.val().length > 0) {
            parent.trigger('change');
        }
    }

    return Select2Cascade;

})(window, $);


var Select2CascadeSubArea = (function (window, $) {

    function Select2CascadeSubArea(parent, child, url, select2Options, selectValue, selectText, placeholderText = '') {
        var afterActions = [];
        var options = select2Options || { width: '100%', theme: 'bootstrap' };

        // Register functions to be called after cascading data loading done
        this.then = function (callback) {
            afterActions.push(callback);
            return this;
        };

        parent.select2(select2Options).on("change", function (e) {
            child.prop("disabled", true);
            var val = child.val();
            if (!(val != null && val.length > 0)) {
                val = child.attr("data-value");
            }
            console.log(val);
            var _this = this;
            $.ajax({
                type: 'GET',
                url: url + "?parentId=" + $(this).val() + "&isAll=1",
                dataType: 'json',
                data: {},
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.msg == "successful") {
                        var newOptions = "";
                        if (placeholderText.length > 0) {
                            newOptions = '<option value="">' + placeholderText + '</option>';
                        } else {
                            newOptions = '<option value="">Tất cả</option>';
                        }
                        for (var i = 0; i < response.content.length; i++) {
                            var disabled = (parseInt(response.content[i].code) > 0 && response.content[i].code.length == 5) ? "" : "disabled=\"disabled\"";
                            newOptions += '<option ' + disabled+' value="' + response.content[i][selectValue] + '">' + response.content[i][selectText] + '</option>';
                        }
                        child.select2('destroy').html(newOptions).prop("disabled", false)
                            .select2(options);
                        if (val != null && val.length > 0) {
                            child.val(val).trigger('change');
                        }
                        afterActions.forEach(function (callback) {
                            callback(parent, child, response.content);
                        });
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                }
            });
        });

        if (parent.val().length > 0) {
            parent.trigger('change');
        }
    }

    return Select2CascadeSubArea;

})(window, $);