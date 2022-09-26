import http from "../../../helpers/axiosClient"

export const getListProduct = ( callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Products/Product/GetAllProduct")
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("getCompany eror:" + response.msg)
            }
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })

}

export const checkGetPointCustomer =  (param, callback) => {
    http.get("/Orders/Point/PointOfCustomer", {params : param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("ApiDetail:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}

export const getListCustomerCoupon =  (param, callback) => {
    http.get("/Orders/Coupon/CouponGetCustomer", {params : param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("ApiDetail:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}

export const getListCustomerCouponEdit =  (param, callback) => {
    http.get("/Orders/Coupon/CouponGetCustomerEdit", {params : param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("ApiDetail:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}

export const getListOrderEdit = (param, callback) => {
    http.get("/Orders/Order/GetListOrderEdit", {params: param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("ApiDetail:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}

export const getPriceProduct  = (param, callback) => {
    http.get("/Orders/Order/GetPriceProduct", {params: param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content)
            } else {
                console.error("CheckShipmentCost:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}
export const getPointOldCustomer =  (param, callback) => {
    http.get("/Orders/Point/PointOldCustomer", {params : param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("PointOldCustomer:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}