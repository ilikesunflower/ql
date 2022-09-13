import React , {useEffect, useState, useRef} from 'react';
import * as Yup from "yup";
import {isHtml, parStr2Float, parInt2Str} from "../common/app"

Yup.addMethod(Yup.mixed, 'maxLength', function (length) {
    return this.test({
        name: 'maxLength',
        message: "Vui lòng chỉ nhập tối đa " + length + " ký tự",
        test: value => {
            try {
                if (value != null && value.length > 0) {
                    if (value.length >= length) {
                        return false;
                    }
                }
                return true;
            }
            catch (e) {
                console.log(e);
                return false;
            }
        }
    });
});
Yup.addMethod(Yup.mixed, 'validHtml', function () {
    return this.test({
        name: 'validHtml',
        message: "Hệ thống không hỗ trợ nhập nội dung html",
        test: value => {
            try{
                return !isHtml(value);
            }catch (e) {
                return false;
            }
        }
    })
})
Yup.addMethod(Yup.mixed, 'positiveNumbers', function (message) {
    return this.test({
        name: 'positiveNumbers',
        message: message,
        test: value => {
            if (!value) {
                return true;
            }
            return parStr2Float(value) >= 0
        }
    });
});
Yup.addMethod(Yup.mixed, 'requiredArray', function (message){
    return this.test({
        name: 'requiredArray',
        message: message,
        test: value => {
            if (!value) {
                return true;
            }
            return (value!= null && value.length > 0)
        }
    });
})
export default Yup;