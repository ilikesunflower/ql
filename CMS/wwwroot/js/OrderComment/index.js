$(function () {
    let toggle = {
        on: 'Hiện',
        off: 'Ẩn',
        size: 'mini',
        onstyle: "success",
        offstyle: "secondary"
    };
    $('.onoffswitch input[type="checkbox"]').bootstrapToggle(toggle);
    $('.ChangeStatus').change(function (e) {
        let status = JSON.parse($(e.currentTarget).attr("data-status").toString());
        let id = $(e.currentTarget).attr("data-id");
        if (status !== $(this).prop('checked')) {
            $(this).prop('checked', status).bootstrapToggle('destroy').bootstrapToggle(toggle);
            Swal.fire({
                title: 'Bạn có chắc chắn muốn thay đổi trạng thái của đánh giá này?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                cancelButtonText: 'Thoát'
            }).then((result) => {
                if (result.value) {
                    let url = $(".url_" + id).attr("href");
                    $(this).closest('.ibox-content').addClass("sk-loading");
                    let dataSend = {
                        id: id,
                        status: status === true ? 0 : 1
                    }
                    console.log("dataSend", dataSend, url);
                    $.ajax({
                        type: 'POST',
                        url: url,
                        contentType: 'application/x-www-form-urlencoded',
                        dataType: 'json',
                        data: dataSend,
                        headers: {
                            RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                        },
                        success: function (response) {
                            $('.ibox-content').removeClass("sk-loading");
                            if (response.msg === "successful") {
                                window.location.reload();
                            } else {
                                toastr.error(response.content, "Thông báo");
                            }
                        },
                        error: function (xhr, textStatus, errorThrown) {
                            $('.ibox-content').removeClass("sk-loading");
                            toastr.error(errorThrown, "Thông báo");
                        }
                    });
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    return false;
                }
            });
        }
    });

})