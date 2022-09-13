$("#notification-form").on('show.bs.dropdown', function (event) {
    $("#spinner-load").css({display: "block"});
    $("#countUnRead").css({display: "none"});
    $("#countUnRead").text(0);
    $.ajax({
        url: '/Admin/Notification/GetNotificationByUser',
        dataType: "json",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        data: {},
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            if(data.msg === "successful"){
                $("#spinner-load").css({display: "none"});
                let listData = data.content.data;
                let countUnRead = Number.parseInt(data.content.countUnRead);
                if (countUnRead > 0){
                    $("#countUnRead").text(countUnRead);
                    $("#countUnRead").css({display: "inline-block"});
                }else{
                    $("#countUnRead").css({display: "none"});
                }
                let html = "";
                if (listData.length > 0){
                    for(let i=0; i < listData.length; i++){
                        let time=listData[i].senderTime;
                        let iconHtml = "";
                        if (listData[i].isUnread==0)
                        {
                            iconHtml = '<span class="float-right text-sm text-primary ">\n' +
                                '<i class="fas fa-circle mr-0 p-l-r-5" style="font-size:0.6rem"></i>\n' +
                                '</span>\n'
                        }
                        else
                        {
                            iconHtml="";
                        }
                        let iconUser="";
                        if (listData[i].senderName ==""||listData[i].senderName == null )
                        {
                            iconUser="";
                        }
                        else
                        {
                            iconUser='<span class="text-sm text-muted font-size-12 align-self-center">\n' +
                                '<i class="far fa-user mr-1"></i> '+listData[i].senderName+'\n'+
                                '</span>\n'
                        }
                        let textTitle=listData[i].title;
                        html += '<a  onclick="redirectNotification(this)" data-link="'+listData[i].link+'" data-id="'+listData[i].id+'" data-isUnRead="'+listData[i].isUnread+'"  class="dropdown-item p-1" style="cursor: pointer" id="notification-item">\n' +
                            '<div class="media align-items-center ">\n' +
                            '<div class="media-body">\n' +
                            '<p class="dropdown-item-title p-2 text-justify font-rem-0-9 font-bold">\n'+ stringTruncate(textTitle,130) +'</p>\n'+
                            '<p class="senderName-time pl-2 pr-2 align-self-center" style="line-height: 24px !important;"> \n'+
                            iconUser +
                            '<span class="float-right text-muted text-sm font-size-12 align-self-center" ><i class="far fa-clock mr-1"></i>'+timeSince(Date.parse(time))+'</span>\n'+
                            '</p>\n'+
                            '</div>\n'+
                            iconHtml +
                            '</div>\n'+
                            '</a>\n'+
                            '<div class="dropdown-divider"></div>';
                    }
                    html += '<div class="dropdown-divider "></div>'+
                        '<h2 onclick="RedirectPageMore()" class="dropdown-item dropdown-footer font-bold  font-rem-1 p-2 m-0 " style="color: rgb(108, 117, 125);cursor: pointer">Xem thêm</h2>';
                }
                else
                {
                    html = '<div class="fa-1x text-center p-2">Không có dữ liệu</div>';
                }
                $(".notification-content").empty();
                $(".notification-content").append(html);
            }
        },
        error: function (xhr) {

        }
    });
});
var stringTruncate = function(str,length){
    let dots = str.length > length ? '...' : '';
    return str.substring(0, length)+dots;
};
//time ago
function timeSince(date) {
    var seconds = Math.floor((new Date() - date) / 1000);
    var interval = seconds / 31536000;
    if (interval > 1)
    {
        return Math.floor(interval) + " năm trước";
    }
    interval = seconds / 2592000;
    if (interval > 1) {
        return Math.floor(interval) + " tháng trước";
    }
    interval = seconds / 86400;
    if (interval > 1) {
        return Math.floor(interval) + " ngày trước";
    }
    interval = seconds / 3600;
    if (interval > 1) {
        return Math.floor(interval) + " giờ trước";
    }
    interval = seconds / 60;
    if (interval > 1) {
        return Math.floor(interval) + " phút trước";
    }
    return Math.floor(seconds) + " giây trước";
}
function redirectNotification(e){
    let id= $(e).attr("data-id");
    $.ajax({
        url: '/Admin/Notification/ReadUserNotification/'+id,
        dataType: "json",
        type: "POST",
        contentType: 'application/json; charset=utf-8',
        headers:
            {
                RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
            },
        data: {},
        async: true,
        processData: false,
        cache: false,
        success: function (data) {
            if(data.msg === "successful"){
            }
        },
        error: function (xhr) {
        }
    });
    // chuyển đến link đích
    let link= $(e).attr("data-link") || "";
    if (link != null && link.length > 0)
    {
        window.location.href = link;
    }
}
function RedirectPageMore(){
    window.location.href = "/Admin/Notification/UsersNotification";
}
window.socket.on("notification",function (name, message) {
    let countUnReadText= $("#countUnRead").text() || "0";
    if (countUnReadText !== "99+")
    {
        let countUnRead = Number.parseInt(countUnReadText);
        countUnRead = countUnRead+1;
        if (countUnRead > 0){
            if (countUnRead > 99){
                $("#countUnRead").text("99+");
            }else{
                $("#countUnRead").text(countUnRead);
            }
            $("#countUnRead").css({display: "inline-block"});
        }
        else
        {
            $("#countUnRead").css({display: "none"});
        }
    }
    // hiển thị toastr
    toastr.success(stringTruncate(message.title,60),name);
});