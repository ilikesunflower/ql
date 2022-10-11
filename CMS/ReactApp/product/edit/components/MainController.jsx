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
    let [listProductPurpose, setListProductPurpose] = useState([]);
    let [listProductCategory, setListProductCategory]  = useState([]);
    let [showPurpose, setShowPurpose]  = useState(false);
    let [showCategory, setShowCategory]  = useState(false);
    let [listFileSave, setListFileSave] = useState([]);
    let [listFile, setListFile] = useState([]);
    let [listFileOld, setListFileOld] = useState([]);
    let [showCropImage, setShowCropImage] = useState(false); 
    let [imageCrop, setImageCrop] = useState('');
    let [indexImage, setIndexImage] = useState(null);
    let [typeImage, setTypeImage] = useState('');
    let [nameI, setNameI] = useState('');

    let [typeFile, setTypeFile] = useState(0);
    let [imageString, setImageString] = useState( '/images/icon/defaultimage.jpg');
    let refI = useRef(null);
    let refImage = useRef(null);
    let [listProperties, setListProperties] = useState([]);
    let [listProperProduct, setListProperProduct] = useState([]);
    let [checkEditPro, setCheckEditPro] = useState(false);
    let [showDeletePurpose, setShowDeletePurpose]  = useState(false);
    let [listPurposeDelete, setListPurposeDelete] = useState([]);
    const id =  window.id;
    useEffect(function () {
        getProductPurpose(function (rs) { 
            setListProductPurpose(rs);
        });
        getProductCategory(function (rs) {
            setListProductCategory(rs);
        });
        let param = {id: window.id}
        getListProduct(param, function (rs) {
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
            formikProduct.setFieldValue("productPurposeId", product.productPurposeId || 0);
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
            setImageString(product.image || '/images/icon/defaultimage.jpg')
        
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
    useEffect(function () {
        let data = [];
        let product = {...listProperProduct};
        let dataC = listProperties.filter(x => x.name != '' && fitterTrimArrayString(x.properties).length != 0) || [];
        
        if(checkEditPro){
            if(Array.isArray(dataC) && dataC.length > 0){
                let count = dataC.length;
                if(count == 1){
                    let  valueP = fitterTrimArrayString(dataC[0].properties);
                    valueP.forEach((obj, index) => {
                        let dataP = {
                            name:  obj?.value,
                            skuMh: product[index]?.skuMh || '',
                            price: 0,
                            quantity: 0
                        }
                        data.push(dataP)
                    })
                }else if(count > 1){
                    
                    let valueP = createListProductProperties(dataC,fitterTrimArrayString(dataC[0].properties),1, count)
                    valueP.forEach((obj, index) => {
                        let dataP = {
                            name:  obj,
                            skuMh:  product[index]?.skuMh || '',
                            price: 0,
                            quantity: 0
                        }
                        data.push(dataP)
                    })
                }
            }
            setListProperProduct(data);   
        }
    }, [listProperties]);
    const fitterTrimArrayString = function (lisObj) {
        let value = lisObj.filter(x => x?.value != '');
        return value;
    }
    const clickElement = function (e) {
        let rs = [...listPurposeDelete, e];
        setListPurposeDelete(rs);
    }
    const deletePurpose = function () {
        let param = {
            "ids" : listPurposeDelete
        };
        if(listPurposeDelete.length > 0) {
            deleteProductPurpose(param, function (response) {
                if (response.code === 200) {
                    setListPurposeDelete([]);
                    if (Array.isArray(response?.dataNoDe) && response?.dataNoDe.length > 0) {
                        let err = "";
                        response?.dataNoDe.forEach(x => {
                            err += x + " . ";
                        })
                        setShowDeletePurpose(!showDeletePurpose); 
                        setListProductPurpose(response.content);
                        toastr.error("Mục đích sử dụng "+ err + "  đã được sử dụng trong sản phẩm khác, không được phép xóa ")

                    } else {
                        setShowDeletePurpose(!showDeletePurpose);
                        setListProductPurpose(response.content);
                        toastr.success("Xóa mục đích sử dụng thành công")
                    }
                } else {
                    toastr.error("Xóa mục đích sử dụng ", "Lỗi")
                }
            })
        }else{
            toastr.error("Vui lòng chọn dữ liệu")
        }
    };
    const handDeletePurpose = function (){
        setShowDeletePurpose(!showDeletePurpose);
    }
    const handPurpose = function () {
        setShowPurpose(!showPurpose);
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
 
    const deleteMany = function (i) {
        let data = [...listFile];
        let data1 = [...listFileSave];
        data.splice(i, 1);
        data1.splice(i, 1);
        setListFile(data);
        setListFileSave(data1);
    }
    const deleteManyOld = function (i) {
        let data = [...listFileOld];
        data.splice(i, 1);
        setListFileOld(data);
    }
//crop imgae cũ
    const cropImage = function (i) { 
        setTypeFile(1);
        let data = [...listFileOld];
        let img = data[i];
        setImageCrop(img);
        let listName = img.split('/').pop();
        let check = listName.split('.').pop();
        setNameI(listName)
        setTypeImage(check);
        setIndexImage(i);
        setShowCropImage(true);
    }
    const handleCropImage = function (event){
        if(!event) return;
        let data = [...listFileOld];
        data.splice(indexImage, 1);
        setListFileOld(data);
        let data1 = [...listFile];
        let data2 = [...listFileSave];
        data2.push(event);
        data1.push(URL.createObjectURL(event));
        setListFileSave(data2);
        setListFile(data1);
        setShowCropImage(false)
    }
    
    //crop image mới
    const cropImageNew = function (i) {
        setTypeFile(2);
        let data = [...listFile];
        let data1 = [...listFileSave];
        let img = data[i];
        let img1 = data1[i].name;

        setImageCrop(img);
        setIndexImage(i);
        setShowCropImage(true);
        let typeImg = img1.split('.').pop();
        let nameF =  img1;
        setNameI(nameF)
        setTypeImage(typeImg);

    }
    const handleCropImageNew = function (event){
        if(!event) return;
        let data = [...listFile];
        let data1 = [...listFileSave];
        data[indexImage] = URL.createObjectURL(event);
        data1[indexImage] = event;
        setListFileSave(data1);
        setListFile(data);
        setShowCropImage(false)
    }
    
    
    const handCategory = function () {
        setShowCategory(!showCategory);
    }
    const handleChangeFile = function (e) {
        let value = [];
        let value1 = [];
        if( e.target.files.length > 0){
            for (let i = 0; i < e.target.files.length; i++) {
                let nameFile = e.target.files[i].name;
                let check = "";
                if (nameFile != "") {
                    check = nameFile.split('.').pop();
                }
                if (check == "jpg" || check == "jpeg" || check == "gif" || check == "png" ) {
                    value.push(e.target.files[i]);
                    value1.push(URL.createObjectURL( e.target.files[i]));
                }else {
                    toastr.error("File không đúng định dạng hình ảnh")
                }
            }
            setListFileSave([...listFileSave,  ...value]);
            setListFile([...listFile,...value1]);
        }
    }
    
    const maxIndexProperties = function (listProperties){
        let indexMax = 0;
        if(Array.isArray(listProperties) && listProperties.length > 0 ){
            let properties = listProperties.reduce(function (previous, current) {
                return (previous.ord > current.ord ? previous : current)
            })
            indexMax = properties.ord + 1;
        }
        return indexMax;
    }

    const addFormProperties = function (){
        let ord = maxIndexProperties(listProperties);
        setCheckEditPro(true);
        let val = {
            ord : ord,
            name: '',
            properties: [{ord: 0, value: ''}]
        }
        setListProperties([...listProperties, val]);
        
    }
    const addDetailProperties = function (e, property){
        setCheckEditPro(true);
        let data =[...listProperties] ;
        let ordNew = maxIndexProperties(property.properties)
        let valueNew = {ord: (ordNew || 1), value: ''}
        let index = data.findIndex(x => x.ord == property.ord);
        data[index].properties.push(valueNew);
        setListProperties(data);

    }
    const handFormProperties1 = function (e, property){
        setCheckEditPro(true);
        let value =  e.target.value.trim();
        let data =[...listProperties] ;
        let check = data.findIndex(x => x.name == value && x.name != '');
        let index = data.findIndex(x => x.ord == property?.ord);

        if(check >= 0 && check != index){
            data.splice(index, 1);
            setListProperties(data);
            toastr.error("Thuộc tính này đã tồn tại")
        }else{
            data[index].name = value;
            setListProperties(data);
        }
        
    }
    const handFormProperties11 = function (e, property, property1){
        setCheckEditPro(true);
        let value = e.target.value.trim();
        if(value != ''){

            let data = [...listProperties];
            let index = data.findIndex(x => x?.ord == property?.ord);
            let check = data[index]?.properties.findIndex(x => x?.value == value);
            let index1 = property.properties.findIndex(x => x?.value == property1?.value);
            if(check > -1 &&  check != index1 ){
                e.target.value = '';
                toastr.error("Thuộc tính này đã tồn tại")
            }else{
                data[index].properties[index1].value = value;
                setListProperties(data);
            }
        }
    }
    const deleteDetailProperties = function (  property1, property2) {
        setCheckEditPro(true);
        let data = [...listProperties];
        let index = data.findIndex(x => x?.ord == property1?.ord);
        let index1 = property1.properties.findIndex(x => x.ord == property2.ord);
        data[index].properties.splice(index1, 1);
        setListProperties(data);
    }
    const handSkuMh = function (e, index) {
       let data = [...listProperProduct];
       let value = e.target.value.trim();
       let check = data.findIndex(x => x.skuMh != '' && x.skuMh == value );
       if(check > -1 && check != index){
           e.target.value= '';
           toastr.error("Mã hàng này đã tồn tại")
       }else{
           data[index].skuMh = value;
           setListProperProduct(data);
       }
    }
    const handPriceSkuMh = function (e, index) {
        let data = [...listProperProduct];
        if(parInt2Str(e.floatValue) >= 0){
            data[index].price = e.floatValue;
            setListProperProduct(data);
        }
    }
    const handPrice = function (e, index) {
        let data = [...listProperProduct];
        if(parInt2Str(e.floatValue) >= 0){
            data[index].price = e.floatValue;
            setListProperProduct(data);
        }
    } 
    const handQuantitySkuMh = function (e, index) {
        let data = [...listProperProduct];
        if(e.floatValue < 999999999 && e.floatValue >= 0){
            data[index].quantity = e.floatValue;
            setListProperProduct(data);
        }
    }
  
    const deleteProperties = function (property){
        setCheckEditPro(true);
        let data =[...listProperties] ;
        let index = data.findIndex(x => x.ord == property.ord);
        data.splice(index, 1);
        setListProperties(data);
    }
    const onClickImage = function () {
        $(refImage.current).click();
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
    const formikProductPurpose = useFormik({
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
    const formikProductCategory = useFormik({
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
                    if(rs.code === 200){
                        setListProductCategory(rs.content);
                        setShowCategory(false);
                        resetForm();
                        toastr.success("Tạo danh mục sản phẩm  thành công")
                    }else if(rs.msg = "same"){
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
   
    return {formik:{formikProduct, formikProductPurpose, formikProductCategory}, 
        state:{
            showPurpose,
            showCategory,
            listProductPurpose,
            listProductCategory,
            listFile,
            refImage,
            refI, imageString,
            listProperties,
            listProperProduct,
            listFileOld,
            showDeletePurpose,
            showCropImage,
            imageCrop,
            typeFile, typeImage, nameI

        },
        method:{
            handPurpose,
            handCategory,
            handleChangeFile,
            onClickImage,
            deleteMany,
            setImageString,
            addFormProperties,
            handFormProperties1,
            handFormProperties11,
            deleteProperties,
            handSkuMh,
            handPriceSkuMh,
            addDetailProperties,
            deleteDetailProperties,
            handPrice, 
            handQuantitySkuMh,
            deleteManyOld,
            clickElement,
            deletePurpose,
            handDeletePurpose,
            cropImage, 
            handleCropImage, 
            cropImageNew, 
            handleCropImageNew,
            setShowCropImage
        } };
}
export default MainController;