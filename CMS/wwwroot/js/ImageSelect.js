(function () {
    $.fn.extend({
        imageSelect: function (options) {
            var defaults = {
                imageLink: '#image-link',
                imageFullink: '#image-full-link'
            };

            var options = $.extend(defaults, options);

            function genId() {
                var d = new Date().getTime();
                var uuid = 'image_xxxx_xxxx'.replace(/[xy]/g, function (c) {
                    var r = (d + Math.random() * 16) % 16 | 0;
                    d = Math.floor(d / 16);
                    return (c == 'x' ? r : (r & 0x7 | 0x8)).toString(16);
                });
                return uuid;
            }

            var id = genId();

            $(options.imageFullink).css('margin-bottom', '10px');

            var element = $(this);
            window[element.attr('id') + '_image_select'] = function (file) {
                $(options.imageLink).val(file.url);
                $(options.imageFullink).attr('src', "/"+file.url).show();
                $('.btnDelete', element).show();
                $('#' + element.attr('id') + '_image_popup').modal('hide');
            }

            return this.each(function () {
                var html =
                    '<div>' +
                    '    <a href="#" class="btn btn-success btn-sm" data-toggle="modal" data-target="#' + element.attr('id') + '_image_popup">Chọn ảnh</a>' +
                    '    <a href="#" class="btn btn-danger btn-sm btnDelete" style="display: none;">Xóa ảnh</a>' +
                    '</div>' +
                    '<div id="' + element.attr('id') + '_image_popup" class="modal fade" role="dialog" style="display: none;">' +
                    '<div class="modal-dialog modal-lg">' +
                    '<div class="modal-content">' +
                    '<div class="modal-header">' +
                    '<button type="button" class="close" data-dismiss="modal">&times;</button>' +
                    '<h4 class="modal-title">Chọn ảnh</h4>' +
                    '</div>' +
                    '<div class="modal-body">' +
                    '<div class="embed-responsive embed-responsive-16by9" style="-webkit-overflow-scrolling: touch !important;">' +
                    '<iframe></iframe>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>';

                element.html(html);

                if ($(options.imageLink).val() != '') {
                    $(options.imageFullink).show();
                    $('.btnDelete', element).show();
                }

                $('#' + element.attr('id') + '_image_popup').on('show.bs.modal', function () {
                    $('iframe', $(this)).attr('src', '/Admin/Files/FilePopup?type=1&Callback=' + element.attr('id') + '_image_select');
                }).on('shown.bs.modal', function () {
                    $('iframe', $(this)).css({
                        width: '100%',
                        height: '100%',
                        border: 'none'
                    });
                });

                $('.btnDelete', element).click(function (e) {
                    e.preventDefault();
                    Swal.fire({
                        title: 'Bạn có chắc chắn xóa file này?',
                        type: 'warning',
                        showCancelButton: true,
                        confirmButtonText: 'Đồng ý',
                        cancelButtonText: 'Thoát'
                    }).then((result) =>  {
                        if (result.value) {
                            $(options.imageLink).val('');
                            $(options.imageFullink).attr('src', '').hide();
                            $('.btnDelete', element).hide();
                        } else if (result.dismiss === Swal.DismissReason.cancel) {
                            return false;
                        }
                    });
                });
            });
        }
    });
})(jQuery);