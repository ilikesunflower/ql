import React , {useEffect, useState, useRef} from 'react';
import Yup from "../../../components/Yup"
import {useFormik} from "formik";
import {isHtml, parStr2Float, parInt2Str} from "../../../common/app"
import {
    saveProduct,
} from "../service/httpService"


function MainController(props) {
    //listFileSave
    let [listFileSave, setListFileSave] = useState([]);
    let [listFile, setListFile] = useState([]);
    //listProperties
    let [listProperties, setListProperties] = useState([]);
    //listProperProduct
    let [listProperProduct, setListProperProduct] = useState([]);
    
    let [imageString, setImageString] = useState( '/images/icon/defaultimage.jpg?w=300');

    const fitterTrimArrayString = function (lisObj) {
        let value = lisObj.filter(x => x?.value != '');
        return value;
    }
    const formikProduct = useFormik({
        initialValues: {
            sku : '',
            name: '',
            weight: 0,
            price: 0,
            priceSale: 0,
            description: '',
            specifications: '',
            lead: '',
            isHot: false,
            isBestSale: false,
            isNew: false,
            isPromotion: false,
            productPurposeId: 0,
            productCategory: [],
            productSex: 0,
            productAge: 0,
            unit : '',
            isPublic: false,
            image: '',
            images:[],
            name1: '',
            name2: '',
            name3: '',
            properties1: [],
            properties2: [],
            properties3: [],
            quantityStock: 0,
            codeStock: '',
            checkExitSku:''
            
        },
        validationSchema: Yup.object().shape({
            sku: Yup.string().required("Vui lòng nhập mã hàng").validHtml().maxLength(255),
            name: Yup.string().required("Vui lòng nhập tên sản phẩm").validHtml().maxLength(255),
            price: Yup.string().positiveNumbers().required("Vui lòng nhập giá bán"),
            priceSale: Yup.string().positiveNumbers().required("Vui lòng nhập giá bán"),
            unit: Yup.string().required("Vui lòng nhập đơn vị"),
            image: Yup.string().required("Vui lòng chọn ảnh"),
            productPurposeId: Yup.number().min(1,"Vui lòng nhập mục đích sử dụng"),
            productAge: Yup.number().min(0, "Vui lòng độ tuổi").lessThan(100,"Độ tuổi phải nhỏ hơn 100"),
            weight: Yup.number().min(1, "Cân nặng phải lớn hơn 1"),
            productCategory: Yup.array().requiredArray("Vui lòng nhập danh mục sản phẩm"),
            quantityStock: Yup.number().lessThan(999999999,"Giá trị tồn kho phải nhỏ hơn 999.999.999"),
            codeStock: Yup.string().test('required', "Vui lòng nhập mã kho hàng", (value) => {
                let checkList = listProperties.filter(x => x.name != '' && fitterTrimArrayString(x.properties).length != 0 );
                return checkList.length > 0 ||   (value != null && value != "");
            } ),
            checkExitSku: Yup.string().test('required', "Vui lòng nhập mã kho hàng", (value) => {
                let checkList = listProperties.filter(x => x.name != '' && fitterTrimArrayString(x.properties).length != 0 );
                if(checkList.length == 0){
                    return  true
                } 
                let checkSku = listProperProduct.filter(x => x.skuMh == "");
                return  checkSku.length == 0;
               
            } ),
        }),
        onSubmit: (values, {resetForm}) => {
            let checkList = listProperties.filter(x => x.name != '' && fitterTrimArrayString(x.properties).length != 0 );
            if(checkList.length == 0){
                setListProperties([]);
            }
            Swal.fire({
                title: 'Bạn muốn lưu sản phẩm ?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                confirmButtonColor: '#ed5565',
                cancelButtonText: 'Thoát'
            }).then((result) => {
                if (result.value) {
                    const formData = new FormData();
                    formData.append("Sku", values.sku.trim());
                    formData.append("Name", values.name.trim());
                    formData.append("Weight",values.weight);
                    formData.append("Price",values.price);
                    formData.append("PriceSale", values.priceSale);
                    formData.append("Description",values.description);
                    formData.append("Lead",values.lead);
                    formData.append("Specifications",values.specifications);
                    formData.append("ProductPurposeId",values.productPurposeId);
                    formData.append("Unit",values.unit);
                    formData.append("Image",values.image);
                    formData.append("IsHot",values.isHot);
                    formData.append("IsNew",values.isNew);
                    formData.append("IsBestSale",values.isBestSale);
                    formData.append("IsPromotion",values.isPromotion);
                    formData.append("ProductSex",values.productSex);
                    formData.append("ProductAge",values.productAge);
                    formData.append("IsPublic",values.isPublic);
                    formData.append("QuantityStock",values.quantityStock);
                    formData.append("CodeStock",values.codeStock);
                    if(Array.isArray(listFileSave) && listFileSave.length > 0){
                        listFileSave.forEach(obj => {
                            formData.append("Images",obj);

                        })
                    }
                    if(Array.isArray(values.productCategory) && values.productCategory.length > 0){
                        values.productCategory.forEach(obj => {
                            formData.append("ProductCategory",obj['value']);
                        })
                    }
                    let checkList = listProperties.filter(x => x.name != '' && fitterTrimArrayString(x.properties).length != 0 );
                    if(Array.isArray(checkList) && checkList.length > 0){
                        checkList.forEach((obj, i) => {
                            formData.append("Name"+(i + 1),obj.name);
                            let nameP = "Properties" + (i + 1);
                            fitterTrimArrayString(obj.properties).forEach(obj1 => {
                                formData.append(nameP, obj1?.value);
                            })
                        })
                    }
                    if(Array.isArray(listProperProduct) && listProperProduct.length > 0){
                        listProperProduct.forEach(obj => {
                            formData.append("ListSkuMh",obj.skuMh);
                            formData.append("ListName",obj.name);
                            formData.append("ListPrice",obj.price);
                            formData.append("ListQuantity",obj.quantity);
                        })
                    }
                    saveProduct(formData, function (rs) {
                        if (rs.code === 200) {
                            window.location.href = "/Products/Product/Details/"+ rs.id;
                            toastr.success("Thêm sản phẩm thành công")
                        } else {
                            if(rs.msg == 'same'){
                                toastr.error("Mã hàng này đã tồn tại! ")
                            }else{
                                toastr.error("Lưu dữ liệu lỗi !")

                            }
                        }
                    })
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    return false;
                }
            })

        }
    })
    return {formikProduct,
        state:{
            listFile, imageString,
            listProperties,
            listProperProduct,
            listFileSave
        },
        method:{
            setImageString,
            setListFileSave,
            setListFile, 
            setListProperties, setListProperProduct
        } };
}
export default MainController;