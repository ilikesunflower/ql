import http from "../../helpers/axiosClient"

export const getChartDataToProduct =  (param, callback) => {
    http.post("/Admin/Home/GetChartToProduct",  param)
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


