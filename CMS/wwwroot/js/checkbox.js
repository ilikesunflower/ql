$('.onoffswitch input[type="checkbox"]').bootstrapToggle({
    on: 'Hiện',
    off: 'Ẩn',
    size: 'lg',
    onstyle: "danger",
    offstyle: "secondary"
});

$('.onoffswitch input[type="checkbox"]').change(function (e) {
    if ($(this).prop("checked") == true) {
        $('input[name="Status"]').val(1);
    } else {
        $('input[name="Status"]').val(0);
    }
});
bindTogge();
function bindTogge() {
    if ($('input[name="Status"]').val() == 1) {
        $('.onoffswitch input[type="checkbox"]').bootstrapToggle('on');
    }
}