function showDetailOrder(date, type){
    if(type == 0){
        let url = "/Orders/Order/IndexOrderCustomerSuccess?startDate=" + date + "&endDate=" + date ;
        window.location.replace(url);
    }
    else if(type == 1){
        let url = "/Orders/Order/IndexOrderCustomerShip?startDate=" + date + "&endDate=" + date ;
        window.location.replace(url);
    }
    else if(type == 2){
        let url = "/Orders/Order/IndexOrderSuccess?startDate=" + date + "&endDate=" + date ;
        window.location.replace(url);
    }
    else if(type == 3){
        let url = "/Orders/Order/IndexOrderCancel?startDate=" + date + "&endDate=" + date ;
        window.location.replace(url);
    }else{
        return;
    }
 
}