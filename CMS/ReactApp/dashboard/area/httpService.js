import http from "../../helpers/axiosClient"

export const getChartArea =  (param, callback) => {
    http.post("/Admin/Home/GetChartArea",  param)
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


