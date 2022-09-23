import React, {useEffect, useState, useMemo} from 'react';
import {formatNumber, isPoi} from '../../../common/app';
import {getListProduct, getListCustomerCoupon, checkGetPointCustomer,getListCustomerCouponEdit } from "./httpService"

function MainController(props) {
    const { productCartSelect, setProductCartSelect, formik,customer, isEdit } = props;
    //product
    let [listProduct, setListProduct] = useState([]);
    let [productSelect, setProductSelect] = useState(0);
    let [showModelDetailProduct, setShowModelDetailProduct] = useState(false);
    //point
    let [customerPoint,setCustomerPoint] = useState(0);
    let [customerPointOld,setCustomerPointOld] = useState(0);
    //coupon
    let [listCoupon, setListCoupon] = useState([]);
    let [showModalCoupon,setShowModalCoupon] = useState(false);


    useEffect(() => {
        getListProduct(function (rs) {
            setListProduct(rs);
        })
    }, []);
    const handleChangeSelect = (event) => {
        setProductSelect(event.value)
    };
    const handShowDetailProduct = () => {
        if(customer == 0 ){
            toastr.error("Vui lòng chọn khách hàng !")

        }else {
            if(productSelect == 0){
                toastr.error("Vui lòng chọn sản phẩm !")
            }else {
                setShowModelDetailProduct(!showModelDetailProduct);
            }
        }
    }
  
    useEffect(function () {
        if(customer != 0){
            checkGetPointCustomer({customerId : customer},function (response) {
                setCustomerPoint(response || 0)
            })
            if(isEdit){
                getListCustomerCouponEdit({customerId : customer}, function (response) {
                    setListCoupon(response);
                }) 
            }else{
                getListCustomerCoupon({customerId : customer}, function (response) {
                    setListCoupon(response);
                })
            }
        }

    },[customer])
    
    //product
    const productTotalPrice = useMemo(() => {
        return productCartSelect.reduce(function (previousValue, currentValue) {
            return previousValue + (currentValue?.price || 0) * (currentValue?.quantityBy || 0);
        } ,0)
    }, [productCartSelect])
    const clickQuantityBuy = (type, index) => {
        let data = [...productCartSelect];
        let check = data[index];
        let quantityBuy = check.quantityBy;
        let quantityWH = check.quantityWH;
        if ( type == 1 ){
            let q = quantityBuy - 1;
            if ( q < 1 ){
                data[index].quantityBy = 1;
            }else{
                data[index].quantityBy = q;

            }
        }else if(type == 2){
            let q = quantityBuy + 1;
            if ( q > quantityWH ){
                data[index].quantityBy = quantityWH;
            }else{
                data[index].quantityBy = q;
            }
        }
        setProductCartSelect(data)

    }
    const changeQuantityBuy = (e, index) => {
        let quantity = Math.floor(e.floatValue);
        let data = [...productCartSelect];
        const quantityWH = data[index].quantityWH;
        if ( quantity > quantityWH ){
            data[index].quantityBy = quantityWH;
        }else if ( quantity >= 1 && quantity <= quantityWH){
            data[index].quantityBy = quantity;
        }else{
            data[index].quantityBy = 1;
        }
        setProductCartSelect(data);

    }
    const deleteProductSelect = (index) => {
        let data = [...productCartSelect];
        data.splice(index, 1);
        setProductCartSelect(data)
    }  
 //point
    const applyPointIncrease = () => {
        let point = formik.values.point;
        let value = point+1;
        if (value > customerPoint){
            toastr.error(`Bạn đang có ${customerPoint} điểm, không thể nhập nhiều hơn`)
            return
        }
        if(value < 0){
            toastr.error(`Bạn không thể nhập số nhỏ hơn 0`)
            return
        }
        formik.setFieldValue("point", value)
    }
    const applyPointDecrease = () => {
        let point = formik.values.point;
        let value = point-1;
        if (value > customerPoint){
            toastr.error(`Bạn đang có ${customerPoint} điểm, không thể nhập nhiều hơn`)
            return
        }
        if(value < 0){
            toastr.error(`Bạn không thể nhập số nhỏ hơn 0`)
            return
        }
        formik.setFieldValue("point", value)
    }
    const applyPoint = (event) => {
        let value = parseInt(event.floatValue)
        if (value > customerPoint){
            toastr.error(`Bạn đang có ${customerPoint} điểm, không thể nhập nhiều hơn`)
            return
        }
        if(value < 0){
            toastr.error(`Bạn không thể nhập số nhỏ hơn 0`)
            return
        }
        formik.setFieldValue("point", value)
    }
    
    //coupon
    const handShowModalCoupon = () => {
        setShowModalCoupon(!showModalCoupon);
    }
    
    //totalPrice
    useEffect(function () {
        let priceShip = formik.values?.priceShip || 0;
        let point = formik.values.point || 0;
        let couponDiscount = formik.values.couponDiscount || 0;
        let total = productTotalPrice + priceShip - point * isPoi - couponDiscount;
        if (total < 0){
            total = 0;
        }
        formik.setFieldValue("total", total);
    },[productTotalPrice,formik.values?.priceShip,formik.values?.point,formik.values?.couponDiscount])
    return {method:{handleChangeSelect, handShowDetailProduct, setProductCartSelect, clickQuantityBuy, changeQuantityBuy, deleteProductSelect
        , applyPointIncrease, applyPointDecrease, applyPoint, handShowModalCoupon}, state: {productTotalPrice, showModalCoupon, listCoupon, listProduct, productCartSelect, customerPoint, productSelect , showModelDetailProduct}};
}

export default MainController;