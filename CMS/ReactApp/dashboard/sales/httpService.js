import http from "../../helpers/axiosClient"

export const getChartDataSales =  (param, callback) => {
    http.post("/Admin/Home/GetChartDataSales",  param)
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


