import http from "../../../helpers/axiosClient"






export const editOrder  = (param, callback) => {
    http.post("/Orders/Order/Edit", param,{
        headers: {
            'Content-Type': 'multipart/form-data'
        }})
        .then(response => {
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


