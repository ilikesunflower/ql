import React , {useEffect, useState, useRef} from 'react';
import Yup from "../../../components/Yup"
import {useFormik} from "formik";
import {isHtml, parStr2Float, parInt2Str} from "../../../common/app"
import {
    getProductPurpose,
    getProductCategory,
    saveProductPurpose,
    saveProductCategory,
    saveProduct,
    deleteProductPurpose
} from "../service/httpService"


function ProductPurposeController(props) {
    //ProductPurpose
    let {formikProduct} = props;
    let [listProductPurpose, setListProductPurpose] = useState([]);
    let [showPurpose, setShowPurpose]  = useState(false);
    let [listPurposeDelete, setListPurposeDelete] = useState([]);
    let [showDeletePurpose, setShowDeletePurpose]  = useState(false);
    const handPurpose = function () {
        setShowPurpose(!showPurpose);
    }
    const handDeletePurpose = function (){
        console.log("handDeletePurpose")
        setShowDeletePurpose(!showDeletePurpose);
    }
    const clickElement = function (e) {
        let rs = [...listPurposeDelete, e];
        setListPurposeDelete(rs);
    }
    const deletePurpose = function () {
        let param = {
            "ids" : listPurposeDelete
        };
        if(listPurposeDelete.length > 0){
            deleteProductPurpose(param, function (response) {
                if (response.code === 200) {
                    setListPurposeDelete([]);
                    if(Array.isArray(response?.dataNoDe) && response?.dataNoDe.length > 0){
                        let err = "";
                        response?.dataNoDe.forEach(x => {
                            err += x + " . ";
                        })
                        setShowDeletePurpose(!showDeletePurpose);
                        setListProductPurpose(response.content);
                        toastr.error("Mục đích sử dụng "+ err + "  đã được sử dụng trong sản phẩm khác, không được phép xóa ")
                    }else{
                        setShowDeletePurpose(!showDeletePurpose);
                        setListProductPurpose(response.content);
                        toastr.success("Xóa mục đích sử dụng thành công")

                    }

                } else {
                    toastr.error("Xóa mục đích sử dụng ", "Lỗi")
                }
            })
        }else {
            toastr.error("Vui lòng chọn dữ liệu")

        }
    };
    useEffect(() => {
        getProductPurpose(function (rs) {
            setListProductPurpose(rs);
        });
    }, []);
    
    useEffect(() => {
        if(formikProduct.values.productPurposeId != "" && formikProduct.values.productPurposeId  != 0){
            let value = formikProduct.values.productPurposeId
            let check = listProductPurpose.findIndex(x => x.Value == value);
            if(check == -1){
                formikProduct.setFieldValue('productPurposeId', 0)
            }
        }
    }, [listProductPurpose])
    const formik = useFormik({
        initialValues: {
            name: ''
        },
        validationSchema: Yup.object().shape({
            name: Yup.string().required("Vui lòng nhập mục đích sử dụng").validHtml().maxLength(255)
        }),
        onSubmit: (values, {resetForm}) => {
            let param = {name: values.name.trim()}
            let index = listProductPurpose.findIndex(x => x.label ==  values.name.trim());
            if(index < 0){
                saveProductPurpose(param, function (rs) {
                    if(rs.code === 200){
                        setListProductPurpose(rs.content);
                        setShowPurpose(false);
                        resetForm();
                        toastr.success("Tạo mục đích sử dụng thành công")
                    }else if(rs.msg = "same"){
                        toastr.error("Mục đích sử dụng đã tồn tại", "Lỗi")  
                    }else{
                        toastr.error("Tạo mục đích sử dụng ", "Lỗi")
                    }
                })
              
            }else {
                toastr.error("Mục đích sử dụng đã tồn tại", "Lỗi")
            }

        }
    })

    return {formik:formik, 
        state:{
            listProductPurpose,
            showPurpose,
            showDeletePurpose
        },
        method:{
            setListProductPurpose,
            setShowPurpose,
            handPurpose,
            handDeletePurpose,
            clickElement,
            deletePurpose,
            
    } };
}
export default ProductPurposeController;