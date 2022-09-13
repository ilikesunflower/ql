$('.read-all-notification').click(function (e) {
    e.preventDefault();
    var url = $(this).attr("formaction");
    Swal.fire({
        title: 'Bạn có muốn đánh dấu đọc hết thông báo',
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
                data: {},
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    window.location.reload();
                },
                error: function (xhr, textStatus, errorThrown) {
                }
            });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            return false;
        }
    });
});

$('.accessLink').click(function () {
    var _self = this;
    var id = $(_self).attr("data-id");
    var link = $(_self).attr("data-link");
    var isUnread = $(_self).attr("data-isunread");
    if (isUnread == "0") {
        var url = $(_self).attr("data-urlread");
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
                    $("#IsUnread_" + id).empty();
                    $("#IsUnread_" + id).html("<small class=\"badge bg-success badge-sm\">Đã đọc</small>");
                    $(_self).attr("data-isunread", "1");
                }
            },
            error: function (xhr, textStatus, errorThrown) {
            }
        });
    }
    if (link != null && link.length > 0) {
        window.location.href = link;
    };
});