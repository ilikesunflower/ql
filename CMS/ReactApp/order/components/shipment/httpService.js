import http from "../../../helpers/axiosClient"

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