$(function () {

    let mediaManager = new MediaManager({
        xsrf: $('input:hidden[name="__RequestVerificationToken"]').val(),
        multiSelect: false,
        type: 1,
        confirmDelete: async (DeleteFile) => {
            let confirm = await Swal.fire({
                title: 'Bạn có chắc chắn xóa dữ liệu này?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                confirmButtonColor: '#ed5565',
                cancelButtonText: 'Thoát'
            });
            if (confirm.value) {
                DeleteFile();
            }
        }
    });

    let mediaManagerForTiny = new MediaManager({
        xsrf: $('input:hidden[name="__RequestVerificationToken"]').val(),
        multiSelect: false,
        confirmDelete: async (DeleteFile) => {
            let confirm = await Swal.fire({
                title: 'Bạn có chắc chắn xóa dữ liệu này?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                confirmButtonColor: '#ed5565',
                cancelButtonText: 'Thoát'
            });
            if (confirm.value) {
                DeleteFile();
            }
        }
    });

    $('img.image-preview').on('click', function () {
        $(this).closest(".file-group").find('.trigger-select-file').trigger("click");
    })

    $('#timepicker').datetimepicker({
        icons: { time: 'far fa-clock' },
        locale: 'vi_VN'
    })

    $(".trigger-select-file").on('click', function () {
        let imagePreview = $(this).closest(".file-group").find('img.image-preview');
        let txtValue = $(this).closest(".file-group").find('input.image-value');
        mediaManager.on('select', function (obj) {
            if (obj.type == 1) {
                imagePreview.attr("src", obj.thumbnail + "?w=350");
                txtValue.val(obj.url);
            } else {
                toastr.error("file không được hỗ trợ. file phải là một hình ảnh.", "Thông báo");
            }
        })
        mediaManager.open();
    })

    $(".trigger-remove-file").on('click', function () {
        let imagePreview = $(this).closest(".file-group").find('img.image-preview');
        let txtValue = $(this).closest(".file-group").find('input.image-value');
        imagePreview.attr("src", '/images/icon/defaultimage.jpg');
        txtValue.val('');
    })


    var optionsTinymce = {
        language: 'vi',
        language_url: '/js/tinymce/langs/vi.js',
        height: 600,
        menubar: true,
        branding: false,
        plugins: [
            'print preview importcss searchreplace autolink autosave save directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime  lists wordcount imagetools textpattern noneditable help charmap  emoticons'
        ],
        toolbar: [
            'formatselect |  fontselect | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | outdent indent | bullist numlist | backcolor forecolor',
            'fontsizeselect | link unlink | removeformat | charmap | superscript subscript | image media link | table | template | code |link'
        ],
        keep_styles: false,
        paste_data_images: true,
        paste_as_text: true,
        convert_urls: false,
        relative_urls: false,
        remove_script_host: false,
        entity_encoding: 'raw',
        valid_elements: '*[*]',
        contextmenu: 'cut copy paste | template',
        content_style: "body { font-family: Open Sans;font-size: 16pt; }",
        file_picker_callback: function (callback, value, meta) {
            mediaManagerForTiny.on('select', function (obj) {
                if (meta.filetype == 'image' && obj.type != 1) {
                    toastr.error("file không được hỗ trợ. file phải là một hình ảnh.", "Thông báo");
                    return;
                }
                if (meta.filetype == 'media' && (obj.type != 2 && obj.type != 3)) {
                    toastr.error("file không được hỗ trợ. file phải là một Media.", "Thông báo");
                    return;
                }
                callback(obj.url, { text: obj.title });
            })
            mediaManagerForTiny.open();
        },
        templates: [
            { title: 'Petrolimex', description: '', url: '/Templates/Tinymce/default.html' }
        ],
        fontsize_formats: '8pt 9pt 10pt 11pt 12pt 14pt 16pt 18pt 20pt 22pt 24pt 26pt 28pt 36pt 48pt 72pt',
        font_formats: "Andale Mono=andale mono,times; " +
            "Arial=arial,helvetica,sans-serif; " +
            "Arial Black=arial black,avant garde;" +
            " Book Antiqua=book antiqua,palatino; " +
            "Comic Sans MS=comic sans ms,sans-serif;" +
            " Courier New=courier new,courier; " +
            "Georgia=georgia,palatino; Helvetica=helvetica; " +
            "Impact=impact,chicago; Symbol=symbol; " +
            "Tahoma=tahoma,arial,helvetica,sans-serif;" +
            " Terminal=terminal,monaco; " +
            "Times New Roman=times new roman,times; " +
            "Trebuchet MS=trebuchet ms,geneva;" +
            " Verdana=verdana,geneva;" +
            " Webdings=webdings;" +
            " Open Sans=Open Sans;" +
            " Wingdings=wingdings,zapf dingbats;" +
            " Noto Serif=noto serif,Segoe UI=segoe ui",
        object_resizing: ':not(.table-media)',
    };

    $('textarea#Detail').tinymce(optionsTinymce);
    $('input[data-role="tagsinput"]').tagsinput('add');
})
