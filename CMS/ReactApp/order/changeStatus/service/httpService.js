import http from "../../../helpers/axiosClient"


export const changeOrderConfirm = (param, callback) => {
    UserInterface.prototype.showLoading();
    http.post("/Orders/Order/ChangeOrderConfirm", param)
        .then(response => {
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })

}

export const statusPayment = (param, callback) => {
    UserInterface.prototype.showLoading();
    http.post("/Orders/Order/StatusPayment", param)
        .then(response => {
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })

}

export const changeOrderShip = (param, callback) => {
    UserInterface.prototype.showLoading();
    http.post("/Orders/Order/ChangeOrderShip", param)
        .then(response => {
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })

}

export const changeOrderSuccess = (param, callback) => {
    UserInterface.prototype.showLoading();
    http.post("/Orders/Order/ChangeOrderSuccess",param)
        .then(response => {
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })

}

export const changeOrderCancel = (param, callback) => {
    UserInterface.prototype.showLoading();
    http.post("/Orders/Order/ChangeOrderCancel",param)
        .then(response => {
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        });
}