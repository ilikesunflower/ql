import http from "../../../../helpers/axiosClient"

export const getListAddressProvince = ( callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Categories/Province/ListAddressProvince")
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("getListAddressProvince eror:" + response.msg)
            }
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading(); 
        })

}
export const getListAddressDistrict = ( param, callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Categories/District/ListAddressDistrict", {params: param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("getListAddressDistrict eror:" + response.msg)
            }
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })

}
export const getListAddressCommune = ( param, callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Categories/Commune/ListAddressCommune", {params: param})
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("getListAddressCommune eror:" + response.msg)
            }
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })

}
