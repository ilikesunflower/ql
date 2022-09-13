
import axios from "axios";
import queryString from "query-string";

const axiosClient = axios.create({
    baseURL: "/",
    headers: {
        "Content-type": "application/json",
        RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
    },
    paramsSerializer: params => queryString.stringify(params)
});



axiosClient.interceptors.request.use(function (config) {
    return config;
}, function (error) {
    return Promise.reject(error);
});

axiosClient.interceptors.response.use(function (response) {
    if (response && !!response.data) {
        return response.data;
    }
    return response;
}, function (error) {
    return Promise.reject(error);
});

export default axiosClient;