$(function() {
    $(".form-upload-info-file,.form-upload-select-file").on("click",function (){
        $("#File").trigger("click");
    })
    $("document").on("drop", function(e) {
        e.stopPropagation();
        e.preventDefault();
    });
    $("#File").on("change",function() {
        let file = this.files[0];
        if (!file){
            $(".form-upload-file").removeClass("has-file");
        }else{
            $(".form-upload-file").addClass("has-file");
            $(".form-upload-file-name").text(file.name);
        }
    });

    $("form.form-upload").on("submit",function(e) {
        e.preventDefault();
        let files = $("#File")[0].files;
        let types = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"];
        if (!files || files.length === 0){
            e.preventDefault();
            UserInterface.prototype.showFlashMessageError("Vui lòng chọn file!");
            return;
        }
        if (!types.includes(files[0].type)){
            e.preventDefault();
            UserInterface.prototype.showFlashMessageError("Hệ thống chỉ hỗ trợ file .xlsx");
            return;
        }
        Swal.fire({
            title: 'Bạn có chắc chắn cập nhật dữ liệu này?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            confirmButtonColor: '#ed5565',
            cancelButtonText: 'Thoát'
        }).then((result) => {
            if (result.value) {
                UserInterface.prototype.showLoading();
                $("form.form-upload").unbind('submit').submit()
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                return false;
            }
        });
    })
})