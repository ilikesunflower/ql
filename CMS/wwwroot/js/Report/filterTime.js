$.fn.datepicker.dates['qtrs'] = {
    days: ["Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7"],
    daysShort: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
    daysMin: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
    months: ["Quý 1", "Quý 2", "Quý 3", "Quý 4", "", "", "", "", "", "", "", ""],
    monthsShort: ["Quý 1", "Quý 2", "Quý 3", "Quý 4", "", "", "", "", "", "", "", ""],
    today: "Today",
    clear: "Clear",
    format: "mm/dd/yyyy",
    titleFormat: "MM yyyy",
    weekStart: 0
};
$('.quarter-year-picker-max').datepicker({
    orientation: 'bottom',
    format: "MM yyyy",
    minViewMode: 1,
    autoclose: true,
    forceParse: false,
    language: "qtrs",
    defaultViewDate: {
        year: Number.parseInt($("#quarterYearTime").attr("data-year")),
        month: Number.parseInt($("#quarterYearTime").attr("data-month"))
    },
    startDate: new Date(new Date().setFullYear(new Date().getFullYear() - 400)),
    endDate: new Date(new Date().setMonth(new Date().getMonth()-3))
}).on("show", function (event) {
    if ($("body").hasClass("layout-navbar-fixed")) {
        var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
        $(".datepicker-dropdown").css("top", top);
    }
    let yearShow = Number.parseInt($(".datepicker-months .datepicker-switch").text() || "0") === (new Date(new Date().setMonth(new Date().getMonth() - 3))).getFullYear();
    // let disableMonth = Math.max(Math.floor((new Date().getMonth() + 2) / 3) - 1,0);
    let disableMonth = Math.floor((new Date().getMonth() + 2) / 3) - 1;
    $(".month").each(function (index, element) {
        if (index > 3) $(element).hide();
        if (yearShow){
            if (index <= disableMonth){
                $(element).removeClass("disabled");
            }else{
                $(element).addClass("disabled");
            }   
        }
    });

});
$('#Type').change(function () {
    let typeTime =  $("#Type").val();
    if (typeTime == "1") {
        $("#monthYearTime").removeClass("hidden").addClass("show");
        $("#quarterYearTime").removeClass("show").addClass("hidden");
        $("#yearTime").removeClass("show").addClass("hidden");
    } else if (typeTime == "2") {
        $("#monthYearTime").removeClass("show").addClass("hidden");
        $("#quarterYearTime").removeClass("hidden").addClass("show");
        $("#yearTime").removeClass("show").addClass("hidden");
    } else if (typeTime == "3") {
        $("#monthYearTime").removeClass("show").addClass("hidden");
        $("#quarterYearTime").removeClass("show").addClass("hidden");
        $("#yearTime").removeClass("hidden").addClass("show");
    }
})

$('#btnChoiceTime').click(function () {
    let typeTime = $("input[name='choiceTime']:checked").val();
    if (typeTime == "1") {
        $("#monthYearTime").removeClass("hidden").addClass("show");
        $("#quarterYearTime").removeClass("show").addClass("hidden");
        $("#yearTime").removeClass("show").addClass("hidden");
    } else if (typeTime == "2") {
        $("#monthYearTime").removeClass("show").addClass("hidden");
        $("#quarterYearTime").removeClass("hidden").addClass("show");
        $("#yearTime").removeClass("show").addClass("hidden");
    } else if (typeTime == "3") {
        $("#monthYearTime").removeClass("show").addClass("hidden");
        $("#quarterYearTime").removeClass("show").addClass("hidden");
        $("#yearTime").removeClass("hidden").addClass("show");
    }
});