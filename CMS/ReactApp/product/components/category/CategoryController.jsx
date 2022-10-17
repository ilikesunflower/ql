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


function CategoryController(props) {
    //CategoryView
    let [listProductCategory, setListProductCategory]  = useState([]);
    let [showCategory, setShowCategory]  = useState(false);
    useEffect(function () {

        getProductCategory(function (rs) {
            setListProductCategory(rs);
        });
    }, []);
    const handCategory = function () {
        setShowCategory(!showCategory);
    }
    const formik = useFormik({
        initialValues: {
            name: ''
        },
        validationSchema: Yup.object().shape({
            name: Yup.string().required("Vui lòng nhập tên danh mục").validHtml().maxLength(255)
        }),
        onSubmit: (values, {resetForm}) => {
            let param = {name: values.name.trim()}
            let index = listProductCategory.findIndex(x => x.label ==  values.name.trim());
            if(index < 0){
                saveProductCategory(param, function (rs) {
                    console.log(rs);
                    if(rs.code === 200){
                        setListProductCategory(rs.content);
                        setShowCategory(false);
                        resetForm();
                        toastr.success("Tạo danh mục sản phẩm  thành công")
                    }else if(rs.msg == "same"){
                        toastr.error("Tạo danh mục sản phẩm đã tồn tại", "Lỗi")
                    }else{
                        toastr.error("Tạo danh mục sản phẩm ", "Lỗi")
                    }
                })

            }else {
                toastr.error("Danh mục sản phẩm đã tồn tại", "Lỗi")
            }

        }
    })
    return {formik:formik, 
        state:{
            listProductCategory,
            showCategory
        },
        method:{
            handCategory
    } };
}
export default CategoryController;