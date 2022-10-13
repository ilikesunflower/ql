import http from "../../../helpers/axiosClient"

export const getListProduct = ( callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Products/Product/GetListProduct")
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("getCompany eror:" + response.msg)
            }
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })

}


export const getProductPurpose = (callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Products/Product/GetProductPurpose")
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("GetProductPurpose eror:" + response.msg)
            }
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })
}

export const getProductCategory = (callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Products/Product/GetProductCategory")
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("GetProductCategory eror:" + response.msg)
            }
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })
}


export const saveProductPurpose = (param, callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Products/Product/SaveProductPurpose", {params: param})
        .then(response => {
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })
}

export const saveProductCategory = (param, callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Products/Product/SaveProductCategory", {params: param})
        .then(response => {
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })
}

export const saveProduct =  (params, callback) => {
    UserInterface.prototype.showLoading();
    http.post("/Products/Product/SaveProduct", params)
        .then(response => {
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })
}

export const getProperties =  ( callback) => {
    UserInterface.prototype.showLoading();
    http.get("/Products/Product/GetListProperties")
        .then(response => {
            if (response.code === 200) {
                callback(response.content);
            } else {
                console.error("lấy properties  eror:" + response.msg)
            }
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })
}

export const deleteProductPurpose= (param, callback) => {
    UserInterface.prototype.showLoading();
    http.post("/Products/Product/DeleteProductPurpose", param)
        .then(response => {
            console.log(response)
            callback(response);
            UserInterface.prototype.hideLoading();
        })
        .catch(e => {
            console.log(e)
            UserInterface.prototype.hideLoading();
        })
}