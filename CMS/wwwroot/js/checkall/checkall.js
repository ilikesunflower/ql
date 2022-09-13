/*	=====================================
 *	jQuery Plugin Check All
 *	Version: 1.0.1
 *
 *	@author: Dinh Viet Bao
 *  @email: baodinh@amitech.vn
 *  @created at: 16/08/2016
 *  @updated at: 28/08/2019
 	=====================================*/

(function () {
    $.fn.extend({
        checkAll: function (options) {
            var defaults = {
                checkAll: '.chkAll',
                checkItem: '.chkItem'
            };

            var options = $.extend(defaults, options);

            return this.each(function () {
                var table = $(this);

                $(options.checkAll, table).on('ifClicked', function () {
                    var check = !this.checked ? 'check' : 'uncheck';
                    $(options.checkItem, table).iCheck(check);
                });

                $(options.checkItem, table).on('ifToggled', function () {
                    var checked_item = $(options.checkItem + ':checked', table).length;
                    var total_item = $(options.checkItem, table).length;
                    var check = checked_item == total_item ? 'check' : 'uncheck';
                    $(options.checkAll, table).iCheck(check);
                });

                var total_item = $(options.checkItem, table).length;
                if (total_item > 0) {
                    var checked_item = $(options.checkItem + ':checked', table).length;
                    var check = checked_item == total_item ? 'check' : 'uncheck';
                    $(options.checkAll, table).iCheck(check);
                } else {
                    $(options.checkAll, table).iCheck('uncheck');
                }
            });
        },

        checkAllVisible: function (options) {
            var defaults = {
                checkAll: '.chkAll',
                checkItem: '.chkItem'
            };

            var options = $.extend(defaults, options);

            return this.each(function () {
                var table = $(this);

                $(options.checkAll, table).on('ifClicked', function () {
                    var check = !this.checked ? 'check' : 'uncheck';
                    $(options.checkItem + '[data-disable!="true"]', table).iCheck(check);
                });

                $(options.checkItem, table).on('ifToggled', function () {
                    var checked_item = $(options.checkItem + '[data-disable!="true"]:checked', table).length;
                    var total_item = $(options.checkItem + '[data-disable!="true"]', table).length;
                    var check = checked_item == total_item ? 'check' : 'uncheck';
                    $(options.checkAll, table).iCheck(check);
                });

                var total_item = $(options.checkItem + '[data-disable!="true"]', table).length;
                if (total_item > 0) {
                    var checked_item = $(options.checkItem + '[data-disable!="true"]:checked', table).length;
                    var check = checked_item == total_item ? 'check' : 'uncheck';
                    $(options.checkAll, table).iCheck(check);
                } else {
                    $(options.checkAll, table).iCheck('uncheck');
                }
            });
        },

        checkAllById: function (options) {
            var defaults = {
                checkAll: '.chkAll',
                checkItem: '.chkItem',
                id: "",
                idcollapse : ""
            };

            var options = $.extend(defaults, options);

            return this.each(function () {
                var table;
                if (options.id != "") {
                     table = $(options.id);
                } else {
                     table = $(this);
                }
                $(options.checkAll).on('ifClicked', function () {
                    var check = !this.checked ? 'check' : 'uncheck';
                    $(options.checkItem, table).iCheck(check);
                    if (options.idcollapse != "") {
                        $(options.idcollapse).CardWidget("toggle");
                    }
                });

                $(options.checkItem, table).on('ifToggled', function () {
                    var checked_item = $(options.checkItem + ':checked', table).length;
                    var total_item = $(options.checkItem, table).length;
                    var check = checked_item == total_item ? 'check' : 'uncheck';
                    $(options.checkAll).iCheck(check);
                });

                var total_item = $(options.checkItem, table).length;
                if (total_item > 0) {
                    var checked_item = $(options.checkItem + ':checked', table).length;
                    var check = checked_item == total_item ? 'check' : 'uncheck';
                    $(options.checkAll).iCheck(check);
                } else {
                    $(options.checkAll).iCheck('uncheck');
                }
            });
        },
    });
})(jQuery);