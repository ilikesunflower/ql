let cropper;
$("#btn-select-image").click(function () {
    $("#file-input").click();
});
$("#file-input").change(function (e) {
    const files = $('#file-input')[0].files[0];
    if (files) {
        const reader = new FileReader();
        reader.onload = function (e) {
            $("#file").html('<img id="img-select"  alt="image"  class="img-circle hidden" src="' +
                e.target.result +
                '" width="250" height="250">');
        };
        reader.readAsDataURL(files);
        setTimeout(initCropper, 300);
    }
    $("#btn-upload-avartar").removeClass("hidden");
    $("#btn-select-image").addClass("hidden");

});
function initCropper(){
    let image = document.getElementById('img-select');
    cropper = new Cropper(image, {
        aspectRatio: 16 / 16,
        viewMode: 1,
        ready: function () {
        },
        // crop: function(e) {
        // }
    });
}
function uuidv4() {
    return ([1e7]+-1e3+-4e3+-8e3+-1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}
$("#btn-upload-avartar").click(function() {
    let url = $("#btn-upload-avartar").attr("data-url");
    let l = $('#btn-upload-avartar').ladda();
    if (cropper){
        const canvas =  cropper.getCroppedCanvas();
        canvas.toBlob(function(blob) {
            let formData = new FormData();
            formData.append('file', blob,uuidv4() +".jpg");
            $.ajax({
                type: 'POST',
                url: url,
                data: formData,
                processData: false,
                contentType: false,
                headers: {
                    RequestVerificationToken:
                        $('input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.msg == "successful") {
                        $("#file").empty();
                        $("#file").html('<img alt="image" class="" src="' +
                            response.detail + '" style="height: 250px;border-radius: 50%;" />');
                        $("#Image").val(response.detail);
                        $("#btn-upload-avartar").addClass("hidden");
                        $("#btn-select-image").removeClass("hidden");
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
                });
        });
    }
});
$("#btn-delete-avartar").click(function () {
    $("#file").empty();
    $('#file-input').val('');
    $("#file").html('<img alt="image" class="img-circle" src="/images/user-default.png" width="250" height="250">');
    $("#Image").val('');
    $("#btn-upload-avartar").addClass("hidden");
    $("#btn-select-image").removeClass("hidden");
});