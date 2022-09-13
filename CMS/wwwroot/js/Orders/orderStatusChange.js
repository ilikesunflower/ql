
$(".orderStatusConfirm").click(function () {
    const url = $(this).attr("formaction");
    Swal.fire({
        title: 'Bạn có chắc chắn xác nhận đơn hàng này?',
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
                data: { },
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.statusCode == 200) {
                        if (urlRedirect.length > 0) {
                            window.location.href = urlRedirect;
                        } else {
                            window.location.reload();
                        }
                    } else {
                        toastr.error(response.message,"Thông báo");
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

$(".orderStatusShip").click(function () {
    const url = $(this).attr("formaction");
    Swal.fire({
        title: 'Bạn có chắc chắn muốn chuyển đơn hàng cho bên vận chuyển này?',
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
                data: { },
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.statusCode == 200) {
                        if (urlRedirect.length > 0) {
                            window.location.href = urlRedirect;
                        } else {
                            window.location.reload();
                        }
                    } else {
                        toastr.error(response.message,"Thông báo");
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

$(".orderStatusSuccess").click(function () {
    const url = $(this).attr("formaction");
    Swal.fire({
        title: 'Bạn có chắc chắn xác nhận hoàn thành đơn hàng này?',
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
                data: { },
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.statusCode == 200) {
                        if (urlRedirect.length > 0) {
                            window.location.href = urlRedirect;
                        } else {
                            window.location.reload();
                        }
                    } else {
                        toastr.error(response.message,"Thông báo");
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

$(".orderStatusCancel").click(function () {
    const url = $(this).attr("formaction");
    Swal.fire({
        title: 'Bạn có chắc chắn hủy đơn hàng này?',
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
                data: { },
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.statusCode == 200) {
                        if (urlRedirect.length > 0) {
                            window.location.href = urlRedirect;
                        } else {
                            window.location.reload();
                        }
                    } else {
                        toastr.error(response.message,"Thông báo");
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