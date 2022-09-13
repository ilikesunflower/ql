$( function() {
    $( "#sortable" ).sortable(
        {
            stop: function(event, ui) {
                let orderOld = ui.item.attr('data-item-sortable-id');
                let valueId = ui.item.attr('data-id');
                var itemOrder = $('#sortable').sortable("toArray", {attribute: 'data-id'});
                var ids = [];
                var ords = [];
                for (var i = 0; i < itemOrder.length; i++) {
                    ids.push(parseInt(itemOrder[i]));
                    ords.push(i);
                }
                let check = ids.findIndex(x => x == valueId);
                if(check != orderOld){
                    console.log(ids, ords)
                    $.ajax({
                        type: 'POST',
                        contentType: 'application/x-www-form-urlencoded',
                        dataType: 'json',
                        headers: {
                            RequestVerificationToken: $('input:hidden[name="__RequestVerificationToken"]').val()
                        },
                        method: "POST",
                        url: "/Categories/ProductCategory/UpdateOrderCategory",
                        data: { Ids: ids, Ords: ords}
                    }).done(function( res ) {
                        console.log(res)
                        if (res.msg === "successful"){
                            console.log(res)
                            //   UserInterface.prototype.showFlashMessageInfo(res.content);
                            document.location.reload(true)
                        }else{
                            UserInterface.prototype.showFlashMessageError(res.message);
                        }
                        UserInterface.prototype.hideLoading();
                    });
                }
                
            }
        }
    );
    $( "#sortable" ).disableSelection();
} );
