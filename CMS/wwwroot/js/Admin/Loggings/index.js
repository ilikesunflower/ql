
function openDetail(e) {
    $("#logging_type").html($(e).attr("data-type").replace(/\'/g, ' '));
    $("#logging_action").html($(e).attr("data-action").replace(/\'/g, ' '));
    $("#logging_detail").html($(e).attr("data-detail").replace(/\'/g, ' '));
    $("#logging_useragent").html($(e).attr("data-useragent").replace(/\'/g, ' '));
    $("#logging_ip").html($(e).attr("data-ip").replace(/\'/g, ' '));
    $("#logging_username").html($(e).attr("data-username").replace(/\'/g, ' '));
    $("#logging_createdat").html($(e).attr("data-createdDate"));
    $("#openDetail").modal({ backdrop: "static" });
}

