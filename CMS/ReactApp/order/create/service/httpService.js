import http from "../../../helpers/axiosClient"




export const createOrder  = (param, callback) => {
    UserInterface.prototype.showLoading();
    http.post("/Orders/Order/Create", param,{
        headers: {
            'Content-Type': 'multipart/form-data'
        }})
        .then(response => {
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e);
            UserInterface.prototype.hideLoading();
        })

}


