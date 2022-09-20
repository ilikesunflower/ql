import http from "../../../helpers/axiosClient"

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