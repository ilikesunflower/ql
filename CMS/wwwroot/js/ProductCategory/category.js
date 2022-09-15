$(function () {

    let nestedSortables = document.getElementsByClassName('list-group-category');
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
                $("> .category-item",evt.to).each(function (){
                    ids.push($(this).attr("data-id"));
                });
                console.log({ ids: ids, parent: parent });
                $.ajax({
                    type: 'POST',
                    contentType: 'application/x-www-form-urlencoded',
                    dataType: 'json',
                    headers: {
                        RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                    },
                    method: "POST",
                    url: "/Categories/ProductCategory/UpdateOrder",
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