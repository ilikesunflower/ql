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
export const getDetailProduct = (param, callback) => {
    http.get("/Products/Product/ApiDetail",{params: param})
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
export const getProductCartList = (param, callback) => {
    http.post("/Orders/Order/GetProductCartList", param)
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
export const checkCouponCustomer  = (param, callback) => {
    http.get("/Orders/Order/CheckCoupon", {params: param})
        .then(response => {
            callback(response);
          
        })
        .catch(e => {
            console.log(e)
        })

}
export const getListCustomer  = ( callback) => {
    http.get("/Orders/Order/GetListCustomer")
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("GetListCustomer:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}
export const getAddressCustomerDefault  = (param, callback) => {
    http.get("/Orders/Order/GetAddressCustomerDefault", {params: param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("GetAddressCustomerDefault:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}

export const checkShipmentCost  = (param, callback) => {
    http.get("/Orders/Shipment/CheckShipmentCost", {params: param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("CheckShipmentCost:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}

export const editOrder  = (param, callback) => {
    http.post("/Orders/Order/Edit", param,{
        headers: {
            'Content-Type': 'multipart/form-data'
        }})
        .then(response => {
            console.log(response)
            callback(response);
        })
        .catch(e => {
            console.log(e)
        })

}


export const getOrderEdit  = (param, callback) => {
    http.get("/Orders/Order/GetOrderEdit", {params: param})
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
export const getCustomer  = (param, callback) => {
    http.get("/Orders/Order/GetCustomer", {params: param})
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
export const getListCustomerCoupon =  (param, callback) => {
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


