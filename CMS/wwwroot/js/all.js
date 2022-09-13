$(".select2").select2({
    width: '100%', theme: 'bootstrap', matcher: function (params, data) {
        // If there are no search terms, return all of the data
        if (jQuery.trim(params.term) === '') {
            return data;
        }
        var myTerm = jQuery.trim(params.term);
        if (data.text.toLowerCase().indexOf(myTerm.toLowerCase()) > -1) {
            return data;
        }
        return null;
    }
});

$('.i-checks').iCheck({
    checkboxClass: 'icheckbox_square-green',
    radioClass: 'iradio_square-green',
});

$('.table-check-all').checkAll();

$('.truncate-name-file').dotdotdot({
    watch: true,
    ellipsis: ' ...',
    height: 50,
    fallbackToLetter: true
});

$(".datepicker").datepicker({
    //    daysOfWeekDisabled: "0",
    forceParse: true,
    calendarWeeks: true,
    autoclose: true,
    format: "dd/mm/yyyy",
    language: 'vi'
}).on("show", function (e) {
    if ($("body").hasClass("layout-navbar-fixed")) {
        var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
        $(".datepicker-dropdown").css("top", top);
    }
});

$(".daterangepicker").daterangepicker({
    "autoUpdateInput": false,
    "singleDatePicker": true,
    "showDropdowns": true,
    "autoApply": true,
    locale: {
        format: 'DD/MM/YYYY',
        "daysOfWeek": [
            "CN",
            "T2",
            "T3",
            "T4",
            "T5",
            "T6",
            "T7",
        ],
        "monthNames": [
            "Tháng 1",
            "Tháng 2",
            "Tháng 3",
            "Tháng 4",
            "Tháng 5",
            "Tháng 6",
            "Tháng 7",
            "Tháng 8",
            "Tháng 9",
            "Tháng 10",
            "Tháng 11",
            "Tháng 12"
        ],
    },
    language: 'vi'
});

$(".year-picker-clear").datepicker({
    orientation: 'bottom',
    format: "yyyy",
    viewMode: "years",
    minViewMode: "years",
    daysOfWeekDisabled: "0",
    forceParse: true,
    calendarWeeks: true,
    autoclose: true,
    clearBtn: true,
    language: 'vi',
}).on("show", function (e) {
    if ($("body").hasClass("layout-navbar-fixed")) {
        var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
        $(".datepicker-dropdown").css("top", top);
    }
});

$(".month-picker").datepicker({
    orientation: 'bottom',
    format: "mm/yyyy",
    viewMode: "months",
    minViewMode: "months",
    daysOfWeekDisabled: "0",
    forceParse: true,
    calendarWeeks: true,
    autoclose: true,
    language: 'vi',
}).on("show", function (e) {
    if ($("body").hasClass("layout-navbar-fixed")) {
        var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
        $(".datepicker-dropdown").css("top", top);
    }
});

$(".year-picker").datepicker({
    orientation: 'bottom',
    format: "yyyy",
    viewMode: "years",
    minViewMode: "years",
    daysOfWeekDisabled: "0",
    forceParse: true,
    calendarWeeks: true,
    autoclose: true,
    language: 'vi',
}).on("show", function (e) {
    if ($("body").hasClass("layout-navbar-fixed")) {
        var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
        $(".datepicker-dropdown").css("top", top);
    }
});

$(".year-picker-max").datepicker({
    orientation: 'bottom',
    format: "yyyy",
    viewMode: "years",
    minViewMode: "years",
    daysOfWeekDisabled: "0",
    forceParse: true,
    calendarWeeks: true,
    autoclose: true,
    language: 'vi',
    startDate: new Date(new Date().setFullYear(new Date().getFullYear() - 400)),
    endDate: new Date(new Date().setFullYear(new Date().getFullYear() - 1))
}).on("show", function (e) {
    if ($("body").hasClass("layout-navbar-fixed")) {
        var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
        $(".datepicker-dropdown").css("top", top);
    }
});
$(".year-picker-max-now").datepicker({
    orientation: 'bottom',
    format: "yyyy",
    viewMode: "years",
    minViewMode: "years",
    daysOfWeekDisabled: "0",
    forceParse: true,
    calendarWeeks: true,
    autoclose: true,
    language: 'vi',
    startDate: new Date(new Date().setFullYear(new Date().getFullYear() - 400)),
    endDate: new Date(new Date().setFullYear(new Date().getFullYear() ))
}).on("show", function (e) {
    if ($("body").hasClass("layout-navbar-fixed")) {
        var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
        $(".datepicker-dropdown").css("top", top);
    }
});
$(".month-picker-max").datepicker({
    orientation: 'bottom',
    format: "mm/yyyy",
    viewMode: "months",
    minViewMode: "months",
    daysOfWeekDisabled: "0",
    forceParse: true,
    calendarWeeks: true,
    autoclose: true,
    language: 'vi',
    startDate: new Date(new Date().setFullYear(new Date().getFullYear() - 400)),
    endDate: new Date(new Date().setMonth(new Date().getMonth() - 1))
}).on("show", function (e) {
    if ($("body").hasClass("layout-navbar-fixed")) {
        var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
        $(".datepicker-dropdown").css("top", top);
    }
});
$(".month-picker-max-now").datepicker({
    orientation: 'bottom',
    format: "mm/yyyy",
    viewMode: "months",
    minViewMode: "months",
    daysOfWeekDisabled: "0",
    forceParse: true,
    calendarWeeks: true,
    autoclose: true,
    language: 'vi',
    startDate: new Date(new Date().setFullYear(new Date().getFullYear() - 400)),
    endDate: new Date(new Date().setMonth(new Date().getMonth()))
}).on("show", function (e) {
    if ($("body").hasClass("layout-navbar-fixed")) {
        var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
        $(".datepicker-dropdown").css("top", top);
    }
});

$(".deleteOne").click(function (e) {
    var url = $(this).attr('formaction');
    var urlRedirect = $(this).attr('data-url-back');
    Swal.fire({
        title: 'Bạn có chắc chắn xóa dữ liệu này?',
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        confirmButtonColor: '#ed5565',
        cancelButtonText: 'Thoát'
    }).then((result) => {
        if (result.value) {
            UserInterface.prototype.showLoading();
            $.ajax({
                type: 'POST',
                url: url,
                dataType: 'json',
                data: {},
                headers: {
                    RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    UserInterface.prototype.hideLoading();
                    if (response.msg === "successful") {
                        if (urlRedirect !== undefined) {
                            window.location.href = urlRedirect;
                        } else {
                            window.location.reload();
                        }
                    } else {
                        window.location.reload();
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    UserInterface.prototype.hideLoading();
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            return false;
        }
    });
});

$('.deleteAll').click(function (e) {
    e.preventDefault();

    var id = [];
    var url = $(this).attr("formaction");
    var urlRedirect = $(this).attr('data-url-back');
    $('.chkItem:checked', $('.table-check-all')).each(function () {
        id.push(parseInt($(this).val()));
    });
    if (id.length === 0) {
        Swal.fire({title: 'Bạn chưa chọn bản ghi.', type: 'warning'});
        return;
    }

    Swal.fire({
        title: 'Bạn có chắc chắn xóa dữ liệu này?',
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        confirmButtonColor: '#ed5565',
        cancelButtonText: 'Thoát'
    }).then((result) => {
        if (result.value) {
            Swal.showLoading();
            $.ajax({
                type: 'POST',
                url: url,
                dataType: 'json',
                data: {id},
                headers: {
                    RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.msg == "successful") {
                        if (urlRedirect.length > 0) {
                            window.location.href = urlRedirect;
                        } else {
                            window.location.reload();
                        }
                    } else {
                        window.location.reload();
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            return false;
        }
    });
});

class UserInterface {
    showFlashMessage(message, type) {
        toastr.options = {
            'closeButton': true,
            'preventDuplicates': false,
            'positionClass': 'toast-top-right',
            'showDuration': '400',
            'hideDuration': '1000',
            'timeOut': '3500',
            'extendedTimeOut': '1000',
            'showEasing': 'swing',
            'hideEasing': 'linear',
            'showMethod': 'fadeIn',
            'hideMethod': 'fadeOut'
        };
        if (type == 1) {
            toastr.success(message, 'Thông báo');
        } else {
            toastr.error(message, 'Thông báo');
        }
    }

    showFlashMessageInfo(message) {
        this.showFlashMessage(message, 1);
    }

    showFlashMessageError(message) {
        this.showFlashMessage(message, 0);
    }

    showLoading() {
        $('#load-spin').removeClass('hidden');
        $('#load-spin').addClass('show-loading');
    }

    hideLoading() {
        $("#load-spin").removeClass('show-loading');
        $("#load-spin").addClass('hidden');
    }

    bindClock(element) {
        if (element.length == 0) {
            return;
        }

        let serverTime = element.attr('data-value');
        // let localTime = new Date();
        // d.setSeconds(d.getSeconds() + 10);
        setInterval(function () {
            let now = new Date(serverTime);
            now.setSeconds(now.getSeconds() + 1);
            serverTime = now;
            element.text(moment(now).format('DD/MM/YYYY HH:mm:ss'));
        }, 1000);
    }

    showAlert(message) {
        Swal.fire({
            title: message,
            icon: 'info',
            allowOutsideClick: false
        });
    }

    showConfirm(message) {
        return Swal.fire({
            title: message,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            confirmButtonColor: '#e55353',
            cancelButtonText: 'Thoát',
            allowOutsideClick: false
        })
    }
}
class Utilities {
    setQueryStringForUrl(url, key, value) {
        let theAnchor = null;
        let newAdditionalUrl = '';
        let tempArray = url.split('?');
        let baseUrl = tempArray[0];
        let additionalUrl = tempArray[1];
        let temp = '';

        if (additionalUrl) {
            let tmpAnchor = additionalUrl.split('#');
            let theParam = tmpAnchor[0];
            theAnchor = tmpAnchor[1];
            if (theAnchor) {
                additionalUrl = theParam;
            }

            tempArray = additionalUrl.split('&');

            for (let i = 0; i < tempArray.length; i++) {
                if (tempArray[i].split('=')[0] != key) {
                    newAdditionalUrl += temp + tempArray[i];
                    temp = '&';
                }
            }
        }
        else {
            let tmpAnchor = baseUrl.split('#');
            let theParam = tmpAnchor[0];
            theAnchor = tmpAnchor[1];

            if (theParam) {
                baseUrl = theParam;
            }
        }

        if (theAnchor) {
            value += '#' + theAnchor;
        }

        let rowTxt = temp + '' + key + '=' + value;
        return baseUrl + '?' + newAdditionalUrl + rowTxt;
    }
    getReplace(stringLy) {
        return stringLy.replace(/\\/g, "")
            .replace(/\\n/g, "\\n")
            .replace(/\\'/g, "\\'")
            .replace(/\\"/g, '\\"')
            .replace(/\\&/g, "\\&")
            .replace(/\\r/g, "\\r")
            .replace(/\\t/g, "\\t")
            .replace(/\\b/g, "\\b")
            .replace(/\\f/g, "\\f");
    }
}


