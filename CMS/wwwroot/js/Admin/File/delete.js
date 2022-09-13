$(".file-checkbox").each(function () {
    if ($(this).find('input[type="checkbox"]').first().attr("checked")) {
        $(this).addClass('file-checkbox-checked');
    }
    else {
        $(this).removeClass('file-checkbox-checked');
    }
});
$(".file-checkbox").on("click", function (e) {
    $(this).toggleClass('file-checkbox-checked');
    var $checkbox = $(this).find('input[type="checkbox"]');
    $checkbox.prop("checked", !$checkbox.prop("checked"));
    e.preventDefault();
});
$(".deleteFileAll").click(function () {
    var id = [];
    $('.chkItem:checked', $('.file-checkbox-all')).each(function () {
        id.push(parseInt($(this).val()));
    });
    var urlRedirect = $(this).attr('data-url-back');
    var url = $(this).attr("formaction");
    if (id.length === 0) {
        Swal.fire({ title: 'Bạn chưa chọn bản ghi.', type: 'warning' });
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
                data: { id },
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
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