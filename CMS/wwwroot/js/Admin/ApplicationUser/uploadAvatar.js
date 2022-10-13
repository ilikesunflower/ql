
$("#btn-upload-avartar").click(function () {
    $("#file-input").click();
});
$("#file-input").change(function (e) {
    var url = $("#btn-upload-avartar").attr("data-url");
    var l = $('#btn-upload-avartar').ladda();
    var files = $('#file-input')[0].files[0];
    if (files != null) {
        l.ladda('start');
        var formData = new FormData();
        formData.append('file', files);
        $.ajax({
            type: 'POST',
            url: url,
            dataType: 'json',
            data: formData,
            cache: false,
            processData: false,
            contentType: false,
            headers: {
                RequestVerificationToken:
                    $('input:hidden[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.msg == "successful") {
                    $("#file").empty();
                    $("#file").html('<img alt="image" class="img-circle" src="' +
                        response.detail + "?w=200" + 
                        '" width="200" height="200">');
                    $("#Image").val(response.detail);
                } else {
                    toastr.error(response.content, "Thông báo");
                }
                $('#file-input').val('');
            },
            error: function (xhr, textStatus, errorThrown) {
                $('#file-input').val('');
            }
        }).always(
            function () {
                l.ladda('stop');
            }
        );
    }
});


$("#btn-delete-avartar").click(function() {
    Swal.fire({
        title: 'Bạn có chắc chắn xóa ảnh đại diện này?',
        type: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        confirmButtonColor: '#ed5565',
        cancelButtonText: 'Thoát'
    }).then(function (result) {
        if (result.value) {
            $("#file").empty();
            $('#file-input').val('');
            $("#file").html('<i class="img-circle fa fa-user-circle-o" style="font-size: 200px"></i>');
            $("#Image").val('');
        }
    });
});