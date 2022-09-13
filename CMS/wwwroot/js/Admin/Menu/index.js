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
                title: 'Bạn có chắc chắn muốn thay đổi trạng thái hiển thị của  menu này?',
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

    let nestedSortables = document.getElementsByClassName('list-group-menu');
    for (let i = 0; i < nestedSortables.length; i++) {
        new Sortable(nestedSortables[i], {
            group: 'nested',
            animation: 150,
            fallbackOnBody: true,
            swap: false,
            swapThreshold: 0.65,
            swapClass: 'highlight',
            filter: '.filtered',
            onEnd: function (evt) {
                if (evt.to ===  evt.from && evt.oldIndex === evt.newIndex){
                    return
                }
                UserInterface.prototype.showLoading();
                let parent = $(evt.to).attr("data-id");
                let ids = [];
                $("> .menu-item",evt.to).each(function (){
                    ids.push($(this).attr("data-id"));
                    // let d = $(this).find(".menu-title");
                    // d.removeClass("menu-level-"+$(this).attr("data-lvl"));
                    // d.addClass("menu-level-"+(lvl+1));
                });
                $.ajax({
                    type: 'POST',
                    contentType: 'application/x-www-form-urlencoded',
                    dataType: 'json',
                    headers: {
                        RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                    },
                    method: "POST",
                    url: "/Admin/Menus/UpdateOrder",
                    data: { ids: ids, parent: parent }
                }).done(function( res ) {
                    if (res.msg === "successful"){
                     //   UserInterface.prototype.showFlashMessageInfo(res.content);
                        document.location.reload(true)
                    }else{
                        UserInterface.prototype.showFlashMessageError(res.message);
                    }
                    UserInterface.prototype.hideLoading();
                });
            },
        });
    }
    
})