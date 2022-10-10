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
    $('.chkItem ').on('ifChanged', function (e) {
        let nameClassChilden = "checkGroup" + this.value;
        setClassChilden(nameClassChilden, e.currentTarget.checked);
        let listClass = $(this).attr('class').split(' ');
        let nameClassParent = listClass.find(x => x.startsWith('checkGroup'));
        setClassParent(nameClassParent, e.currentTarget.checked)
    })
})
function setClassChilden(nameClass, bon) {
    $("." + nameClass ).each(function (e){
        if(bon){
            $(this).closest('.icheckbox_square-green').addClass('checked')
            $(this)[0].checked = bon;
            let nameClassC = "checkGroup" + this.value;
            setClassChilden(nameClassC, bon) 
        }else{
            $(this).closest('.icheckbox_square-green').removeClass('checked')
            $(this)[0].checked = bon;
            let nameClassC = "checkGroup" + this.value;
            setClassChilden(nameClassC, bon)
        }
      
    });
}
function setClassParent(nameClass, bon){
    let parent = nameClass.replace("checkGroup", "");
    let parentId = Number.parseInt(parent || 0);
    if(parentId> 0){
        if(bon){
            let childenTrue =   $("." + nameClass + ":checked"  ).length;
            let childen =  $("." + nameClass  ).length;
            if(childenTrue == childen){
                $(".chkItem[value='"+ parentId +"']").closest('.icheckbox_square-green').addClass('checked');
                $(".chkItem[value='"+ parentId +"']")[0].checked = bon;
                let listClass =   $(".chkItem[value='"+ parentId +"']").attr('class').split(' ');
                let nameClassParent = listClass.find(x => x.startsWith('checkGroup'));
                setClassParent(nameClassParent, bon)
            }
        }
        else{
            
            let listC =  $(".chkItem[value='"+ parentId +"']").closest('.icheckbox_square-green').attr('class').split(' ');
            let check = listC.findIndex(x => x == "checked");
            if(check > -1){
                $(".chkItem[value='"+ parentId +"']").closest('.icheckbox_square-green').removeClass('checked');
                $(".chkItem[value='"+ parentId +"']")[0].checked = bon;
                let listClass =   $(".chkItem[value='"+ parentId +"']").attr('class').split(' ');
                let nameClassParent = listClass.find(x => x.startsWith('checkGroup'));
                setClassParent(nameClassParent, bon)

            }
        
        }
    }
}