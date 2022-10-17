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
    getListProduct,
    deleteProductPurpose,
    saveProductEdit
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
    
    let [listFileOld, setListFileOld] = useState([]);

    let [checkEditPro, setCheckEditPro] = useState(false);


 
    const id =  window.id;
    useEffect(function () {
  
        let param = {id: window.id}
        getListProduct(param, function (rs) {
            console.log("content1", rs.content1)
            let product = rs.content1;
            formikProduct.setFieldValue("sku", product.sku);
            formikProduct.setFieldValue("name", product.name);
            formikProduct.setFieldValue("weight", product.weight || 0);
            formikProduct.setFieldValue("price", product.price ?? 0);
            formikProduct.setFieldValue("priceSale", product.priceSale || 0);
            formikProduct.setFieldValue("description", product.description || '');
            formikProduct.setFieldValue("specifications", product.specifications || '');
            formikProduct.setFieldValue("isHot", product.isHot || false);
            formikProduct.setFieldValue("isNew", product.isNew || false);
            formikProduct.setFieldValue("isPromotion", product.isPromotion || false);
            formikProduct.setFieldValue("productPurposeId", product?.productPurposeId ?? 0);
            formikProduct.setFieldValue("productCategory", []);
            formikProduct.setFieldValue("productSex", product.productSex || 0);
            formikProduct.setFieldValue("productAge", product.productAge || 0);
            formikProduct.setFieldValue("unit", product.unit || 0);
            formikProduct.setFieldValue("isPublic", product.isPublic || false);
            formikProduct.setFieldValue("isBestSale", product.isBestSale || false);
            formikProduct.setFieldValue("image", product.image || '');
            formikProduct.setFieldValue("description", product.description || '');
            formikProduct.setFieldValue("specifications", product.specifications || '');
            formikProduct.setFieldValue("lead", product.lead || '');
            setImageString(product.image ? product.image  + "?w=350" : '/images/icon/defaultimage.jpg')
        
            let proties = rs.content2;
            if(Array.isArray(proties) && proties.length > 0){
                let data = [];
                proties.forEach((obj, i) => {
                    let val = {
                        ord: i,
                        name: obj.name,
                        properties: Array.isArray( obj.listValueName) ?  obj.listValueName.map((x , index) => {
                            return { ord: index, value: x}
                        }) : []
                    }
                    data.push(val);
                })
                setListProperties(data);
            }
            
            let sima = rs.content3;
            if(Array.isArray(proties) &&  proties.length > 0) {
                if(Array.isArray(sima) && sima.length > 0){
                    let data = [];
                    sima.forEach((obj, i) => {
                        let val = {
                            name:  obj.name,
                            skuMh: obj.skuwh || '',
                            price: obj.price,
                            quantity: obj.quantityWh
                        }
                        data.push(val);
                    })
                    console.log("productProperties lần 1",data )
                    setListProperProduct(data);
                }
            }else{
                formikProduct.setFieldValue("quantityStock", sima[0].quantityWh || 0);
                formikProduct.setFieldValue("codeStock", sima[0].skuwh || '');
                formikProduct.setFieldValue("price", sima[0].price || 0);
            }
            
            let category = rs.content4;
            formikProduct.setFieldValue("productCategory", category|| []);
            let listImage = rs.content5;
            setListFileOld(listImage);
        })

    }, []);
    const fitterTrimArrayString = function (lisObj) {
        let value = lisObj.filter(x => x?.value != '');
        return value;
    }


    const createListProductProperties = (listPs, listPro, index2, count) => {
        let pro2 = fitterTrimArrayString(listPs[index2].properties);
        let properties = [];
        listPro.forEach(obj => {
            pro2.forEach(obj2 => {
                properties.push((index2 == 1 ? obj?.value  : obj )+ ' _ ' + obj2?.value );
            })
        })
        let indexNext = index2 + 1;

        if(indexNext == count){
            return properties;
        }else{
            return createListProductProperties(listPs,properties, indexNext , count);
        }
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
            isNew: false,
            isPromotion: false,
            productPurposeId: 0,
            productCategory: [],
            productSex: 0,
            productAge: 0,
            unit : '',
            isPublic: true,
            isBestSale: false,
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
            checkExitSku: ''
        },
        validationSchema: Yup.object().shape({
            sku: Yup.string().required("Vui lòng nhập mã hàng").validHtml().maxLength(255),
            name: Yup.string().required("Vui lòng nhập tên sản phẩm").validHtml().maxLength(255),
            weight:Yup.number().min(1, "Vui lòng cân nặng"),
            price: Yup.string().positiveNumbers().required("Vui lòng nhập giá bán"),
            priceSale: Yup.string().positiveNumbers().required("Vui lòng nhập giá bán"),
            unit: Yup.string().required("Vui lòng nhập đơn vị"),
            image: Yup.string().required("Vui lòng chọn ảnh"),
            productPurposeId: Yup.number().min(1,"Vui lòng nhập mục đích sử dụng"),
            productAge: Yup.number().min(0, "Vui lòng độ tuổi").lessThan(100,"Độ tuổi phải nhỏ hơn 100"),
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
            Swal.fire({
                title: 'Bạn muốn sửa sản phẩm ?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                confirmButtonColor: '#ed5565',
                cancelButtonText: 'Thoát'
            }).then((result) => {
                if (result.value) {
                    const formData = new FormData();
                    formData.append("Id", id);
                    formData.append("Sku", values.sku.trim());
                    formData.append("Name", values.name.trim());
                    formData.append("Weight",values.weight);
                    formData.append("Price",values.price);
                    formData.append("PriceSale", values.priceSale);
                    formData.append("Description",values.description);
                    formData.append("Specifications",values.specifications);
                    formData.append("Lead",values.lead);
                    formData.append("ProductPurposeId",values.productPurposeId);
                    formData.append("Unit",values.unit.trim());
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
                    formData.append("CheckEdit",checkEditPro);
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
                    if(Array.isArray(listFileOld) && listFileOld.length > 0){
                        listFileOld.forEach(obj => {
                            formData.append("ImageList",obj);
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
                            formData.append("ListSkuMh",obj.skuMh || '');
                            formData.append("ListName",obj.name);
                            formData.append("ListPrice",obj.price);
                            formData.append("ListQuantity",obj.quantity);
                        })
                    }
                    saveProductEdit(formData, function (rs) {
                        if (rs.code === 200) {
                            window.location.href = "/Products/Product/Details/"+ rs.id;
                            toastr.success("Sửa sản phẩm thành công")
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
           imageString,
            listProperties,
            listProperProduct,
            listFileOld,
            listFileSave,
            checkEditPro, 

        },
        method:{
            setImageString,
            setListFileSave, setListProperties,
             setListFileOld,
            setListFile,
            setListProperProduct,
            setCheckEditPro
        } };
}
export default MainController;