"use strict"
$(function () {
    $("#ApprovedPrice").on("click",function () {
        let id = $(this).attr("data-id");
        let url = "/Products/ProductCensorship/ApprovedPrice/"+id;
        Swal.fire({
            title: 'Bạn có chắc chắn muốn duyệt giá sản phẩm này?',
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
                        if (response.code === 200) {
                            window.location.reload();
                        } else {
                        }
                    },
                });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                return false;
            }
        });
    })
    $("#ApprovedContent").on("click",function () {
        let id = $(this).attr("data-id");
        let url = "/Products/ProductCensorship/ApprovedContent/"+id;
        Swal.fire({
            title: 'Bạn có chắc chắn muốn duyệt nội dung sản phẩm này?',
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
                        if (response.code === 200) {
                            window.location.reload();
                        }
                    }
                });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                return false;
            }
        });
    })
    $("#ApprovedImage").on("click",function () {
        let id = $(this).attr("data-id");
        let url = "/Products/ProductCensorship/ApprovedImage/"+id;
        Swal.fire({
            title: 'Bạn có chắc chắn muốn duyệt hình ảnh, màu sắc, thương hiệu sản phẩm này?',
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
                        if (response.code === 200) {
                            window.location.reload();
                        }
                    }
                });
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                return false;
            }
        });
    })
})