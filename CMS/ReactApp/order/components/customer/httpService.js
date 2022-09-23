import http from "../../../helpers/axiosClient"

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
