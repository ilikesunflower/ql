
Dropzone.autoDiscover = false;
var maxSize = $("#maxFileSize").val();
$('#dropzoneForm').dropzone({
    url: '/Admin/File/UploadFile',
    paramName: 'files',
    uploadMultiple: true,
    timeout: 0,
    maxFiles: 10,
    parallelUploads: 10,
    autoDiscover: false,
    addRemoveLinks: true,
    createImageThumbnails: true,
    maxFilesize: maxSize,
    headers: { RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val() },
    dictDefaultMessage: 'Kéo file hoặc Bấm vào đây để thực hiện Upload.',
    uploadprogress: function (file, progress, bytesSent) {
        var progressElement = $('[data-dz-uploadprogress]', file.previewElement);
        progressElement.css('width', progress + '%');
        if (file.upload.progress == 100) {
            $(file.previewElement).append('<div class="dz-converting text-center"><img src="/images/loader/loading2.gif" style="width:16px" /> Đang xử lý...</div>');
        }
    },
    acceptedFiles: $("#acceptedFiles").val(),
    init: function () {
        this.on("complete", function (file) {
            var progressElement = file.previewElement.querySelector(".dz-converting");
            $(progressElement).remove();
        });
    },
    success: function (file, response) {
        if (response.msg == "successful") {
            var s = "<div style='font-size: 13px;'>";
            for (var i = 0; i < response.content.length; i++) {
                s += response.content[i];
            }
            s += "</div>";
            Swal.fire({
                title: 'Thông báo!',
                html: s,
                type: 'success',
                confirmButtonText: 'OK'
            }).then(function(result) {
                if (result.value) {
                    location.reload();
                }
            });
        } else {
            Swal.fire({
                title: 'Thông báo!',
                text: 'Lỗi không thể upload file: ' + response.detail,
                type: 'error',
                confirmButtonText: 'OK'
            }).then(function (result) {
                if (result.value) {
                }
            });
        }
    },
    error: function (file, response) {
        var drop = this;
        if (response.indexOf('File is too big') !== -1) {
            toastr.error("Upload file " + file.name+" quá dung lượng: " + maxSize + "MB", "Thông báo");
            drop.removeFile(file);
        } else if (response.indexOf('You can\'t upload files of this type.') !== -1) {
            toastr.error('Không thể upload do file ' + file.name +' không đúng định dạng được hỗ trợ', "Thông báo");
            drop.removeFile(file);
        } else {
            toastr.error('Upload file '+file.name+' lỗi: ' + response, "Thông báo");
            drop.removeFile(file);
        }
    }
});

function openModalFileDetail(name,token,size,date) {
    $("#modal_file_name").text(name);
    $("#fileToken").val(token);
    $("#modal_file_size").text(size);
    $("#modal_file_time").text(date);
    $("#modal-file-detail").modal({ backdrop: "static" });
}

function downfile() {
    var url = "/Admin/File/GetDownloadFile?url=" + window.location.href+"&token=" + $("#fileToken").val();
    window.open(url, '_self');
}

function downFileIndex(token) {
    var url = "/Admin/File/GetDownloadFile?url=" + window.location.href + "&token=" + token;
    window.open(url, '_self');
}