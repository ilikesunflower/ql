"use strict"

$(function () {
    $("#cancel-order").on("click",function (e) {
        console.log($(this))
        let id = $(this).attr("data-id");
        Swal.fire({
            title: 'Bạn có chắc chắn muốn hủy Pre Order?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Thoát',
            confirmButtonColor: '#ed5565'
        }).then((result) => {
            if (result.value) {
                let url = "/Orders/PreOrder/Cancel/"+id;
                $.ajax({
                    type: 'POST',
                    url: url,
                    dataType: 'json',
                    data: { },
                    headers: {
                        RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                    },
                    success: function (response) {
                        if (response.msg === 'successful') {
                            window.location.reload();
                        } else {
                            toastr.error(response.message,"Thông báo");
                        }
                    },
                    error: function (xhr, textStatus, errorThrown) {
                        toastr.error(errorThrown,"Thông báo");
                    }
                });
            }
        });
    })
})