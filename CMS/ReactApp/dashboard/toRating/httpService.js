
import http from "../../helpers/axiosClient"

export const getToRating =  (param, callback) => {
    http.post("/Admin/Home/GetToRating",  param)
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


