import http from "../helpers/axiosClient"



export const getCouponCustomer =  (param, callback) => {
    http.get("/Customer/Customer/GetCouponCustomer", {params : param})
        .then(response => {
            if (response.code === 200) {
                callback(response);
            } else {
                console.error("ApiDetail:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}


