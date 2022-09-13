
$("select[name='ControllerId']").select2({}).on("select2:select", function (e) {
    bindAction($(e.currentTarget));
});
function bindAction(e) {
    var controllerId = e.val();
    if (controllerId > 0) {
        var actionstring = e.find('option[value="' + controllerId + '"]').attr("data-action");
        var actionList = JSON.parse(actionstring);
        $("select[name='ActionId']").empty();
        if (actionList.length == 0) {
            $("select[name='ActionId']").append(new Option("#", "", true, true)).trigger('change');
        }
        for (var i = 0; i < actionList.length; i++) {
            var newOption = new Option(actionList[i].text, actionList[i].id, true, true);
            $("select[name='ActionId']").append(newOption).trigger('change');
        }
    } else {
        $("select[name='ActionId']").empty().append(new Option("#", "", true, true)).trigger('change');;
    }
}
$('.onoffswitch input[type="checkbox"]').bootstrapToggle({
    on: 'Hiện',
    off: 'Ẩn',
    size: 'mini',
    onstyle: "success",
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