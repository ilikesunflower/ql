import http from "../../helpers/axiosClient"

export const getChartDataSales =  (params, callback) => {
    http.get("/Reports/CustomerActivity/WidgetDashboard",{params})
        .then(response => {
            if (response.msg === "successful") {
                callback(response.content);
            } else {
                console.error("ApiDetail:" + response.msg)
            }
        })
        .catch(e => {
            console.log(e)
        })

}


