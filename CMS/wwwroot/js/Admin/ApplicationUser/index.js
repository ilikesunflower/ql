$(".btnSyncLdap").click(function (e) {
    let url = $(this).attr('formaction');
    Swal.fire({
        title: 'Bạn có chắc chắn muốn cập nhật dữ liệu này?',
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        confirmButtonColor: '#ed5565',
        cancelButtonText: 'Thoát'
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: 'POST',
                url: url,
                dataType: 'json',
                data: {},
                headers: {
                    RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.msg == "successful") {
                        window.location.reload();
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