import React, {useEffect, useState, useMemo} from 'react';
import {formatNumber, isPoi} from '../../../common/app';
import {getListProduct, getListCustomerCoupon, checkGetPointCustomer,getListCustomerCouponEdit, getListOrderEdit, getPriceProduct, getPointOldCustomer } from "./httpService"

function MainController(props) {
    const { productCartSelect, setProductCartSelect, formik,customer,orderId, isEdit } = props;
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
        if(isEdit){
            getListOrderEdit({id: orderId}, function (rs) {
                setProductCartSelect(rs);
          
            })
            getPointOldCustomer({orderId: orderId}, function (rs) {
                console.log(rs);
                setCustomerPointOld(rs);
            })
        }
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
                    console.log("getListCustomerCoupon", response)
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
        if(isEdit && check?.old){
                    if(  check?.change ){
                        Swal.fire({
                            title: 'Giá đã thay đổi bạn có muốn thay đổi số lượng không ?',
                            type: 'warning',
                            showCancelButton: true,
                            confirmButtonText: 'Đồng ý',
                            confirmButtonColor: '#ed5565',
                            cancelButtonText: 'Thoát'
                        }).then((result) => {
                            if (result.value) {
                                let quantityBuyOld = check?.quantityByOld || 0;
                                let quantityWH = check?.quantityWH || 0;
                                let quantityBuy = check.quantityBy;
                                data[index].price = check?.priceNew || 0;
                                data[index].weight = check?.weightNew || 0;
                                if ( type == 1 ){
                                    let q = quantityBuy - 1;
                                    if ( q < 1 ){
                                        data[index].quantityBy = 1;
                                    }else{
                                        data[index].quantityBy = q;
                                    }

                                }else if(type == 2){
                                    let q = quantityBuy + 1;
                                    if ( q > (quantityWH + quantityBuyOld) ){
                                        data[index].quantityBy = quantityWH + quantityBuyOld;
                                    }else{
                                        data[index].quantityBy = q;
                                    }
                                }
                                setProductCartSelect(data);

                            } else if (result.dismiss === Swal.DismissReason.cancel) {
                                return false;
                            }
                        })
                    }
                    else{
                        let quantityBuy = check.quantityBy;
                        let quantityBuyOld = check?.quantityByOld || 0;
                        let quantityWH = check?.quantityWH || 0;
                        if ( type == 1 ){
                            let q = quantityBuy - 1;
                            if ( q < 1 ){
                                data[index].quantityBy = 1;
                            }else{
                                data[index].quantityBy = q;
                            }
                        }else if(type == 2){
                            let q = quantityBuy + 1;
                            if ( q > ( quantityWH + quantityBuyOld) ){
                                data[index].quantityBy = quantityWH + quantityBuyOld;
                            }else{
                                data[index].quantityBy = q;
                            }
                        }
                        setProductCartSelect(data);

                    }

               
        }else{

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
    }
    const changeQuantityBuy = (e, index) => {
        let quantity = parseInt(e.value) ;
        let data = [...productCartSelect];
        let check = data[index];
        if(isEdit  && check?.old){
           
                if( check?.change){
                    Swal.fire({
                        title: 'Giá đã thay đổi bạn có muốn thay đổi số lượng không ?',
                        type: 'warning',
                        showCancelButton: true,
                        confirmButtonText: 'Đồng ý',
                        confirmButtonColor: '#ed5565',
                        cancelButtonText: 'Thoát'
                    }).then((result) => {
                        if (result.value) {
                            let quantityWH = check?.quantityWH || 0;
                            let quantityBuy = check.quantityByOld;
                            data[index].price = check?.priceNew || 0;
                            data[index].weight = check?.weightNew || 0;
                            if ( quantity > (quantityWH + quantityBuy) ){
                                console.log("lớn hơn",quantityWH + quantityBuy, quantity )
                                data[index].quantityBy = quantityWH + quantityBuy;
                            }else if ( quantity >= 1 && quantity <= (quantityWH + quantityBuy)){
                                data[index].quantityBy = quantity;
                            }else{
                                data[index].quantityBy = 1;
                            }
                            setProductCartSelect(data);

                        } else if (result.dismiss === Swal.DismissReason.cancel) {
                            return false;
                        }
                    })
                }
                else{
                    let quantityBuy = check.quantityByOld;
                    let quantityWH = check.quantityWH;

                    if ( quantity > (quantityWH + quantityBuy) ){
                        console.log("lớn hơn",quantityWH + quantityBuy, quantity )
                        data[index].quantityBy = quantityWH + quantityBuy;
                    }else if ( quantity >= 1 && quantity <= (quantityWH + quantityBuy)){
                        data[index].quantityBy = quantity;
                    }else{
                        data[index].quantityBy = 1;
                    }
                    setProductCartSelect(data);

                }
        }else{

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


    }
    const deleteProductSelect = (index) => {
        let data = [...productCartSelect];
        data.splice(index, 1);
        setProductCartSelect(data)
    }  
    
 //point
    const applyPointIncrease = () => {
        if(isEdit){
            if( !formik.values.checkChangePoi ){
                Swal.fire({
                    title: 'Số điểm của bạn có thể bị thay đổi bạn có chắc chắn sửa  ?',
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Đồng ý',
                    confirmButtonColor: '#ed5565',
                    cancelButtonText: 'Thoát'
                }).then((result) => {
                    if (result.value) {
                        let point1 = formik.values.point;
                        let value = point1+1;
                        formik.setFieldValue("checkChangePoi", true)
                        if (value > (customerPoint + customerPointOld)){
                            formik.setFieldValue("point", customerPoint + customerPointOld)
                            toastr.error(`Bạn chỉ có ${customerPoint + customerPointOld} không thể nhập nhiều hơn`)
                            return
                        }
                        if(value < 0){
                            formik.setFieldValue("point", 0)
                            toastr.error(`Bạn không thể nhập số nhỏ hơn 0`)
                            return
                        }
                        formik.setFieldValue("point", value)
                    } else if (result.dismiss === Swal.DismissReason.cancel) {
                        return false;
                    }
                })
            }else{
                let point1 = formik.values.point;
                let value = point1+1;
                formik.setFieldValue("checkChangePoi", true)
                if (value > (customerPoint + customerPointOld)){
                    formik.setFieldValue("point", customerPoint + customerPointOld)
                    toastr.error(`Bạn chỉ có ${customerPoint + customerPointOld} không thể nhập nhiều hơn`)
                    return
                }
                if(value < 0){
                    formik.setFieldValue("point", 0)
                    toastr.error(`Bạn không thể nhập số nhỏ hơn 0`)
                    return
                }
                formik.setFieldValue("point", value)
            }

        }else {
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
    }
    const applyPointDecrease = () => {
        if(isEdit){
            if( !formik.values.checkChangePoi ) {
                Swal.fire({
                    title: 'Số điểm của bạn có thể bị thay đổi bạn có chắc chắn sửa  ?',
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Đồng ý',
                    confirmButtonColor: '#ed5565',
                    cancelButtonText: 'Thoát'
                }).then((result) => {
                    if (result.value) {
                        let point1 = formik.values.point;
                        let value = point1 - 1;
                        formik.setFieldValue("checkChangePoi", true)
                        if (value > (customerPoint + customerPointOld)) {
                            formik.setFieldValue("point", customerPoint + customerPointOld)
                            toastr.error(`Bạn chỉ có ${customerPoint + customerPointOld} không thể nhập nhiều hơn`)
                            return
                        }
                        if (value < 0) {
                            formik.setFieldValue("point", 0)
                            toastr.error(`Bạn không thể nhập số nhỏ hơn 0`)
                            return
                        }
                        formik.setFieldValue("point", value)
                    } else if (result.dismiss === Swal.DismissReason.cancel) {
                        return false;
                    }
                })
            }else{
                let point1 = formik.values.point;
                let value = point1 - 1;
                formik.setFieldValue("checkChangePoi", true)
                if (value > (customerPoint + customerPointOld)) {
                    formik.setFieldValue("point", customerPoint + customerPointOld)
                    toastr.error(`Bạn chỉ có ${customerPoint + customerPointOld} không thể nhập nhiều hơn`)
                    return
                }
                if (value < 0) {
                    formik.setFieldValue("point", 0)
                    toastr.error(`Bạn không thể nhập số nhỏ hơn 0`)
                    return
                }
                formik.setFieldValue("point", value)
            }

        }else{
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
    }
    const applyPoint = (event) => {
        if(isEdit){
            if( !formik.values.checkChangePoi ){
                Swal.fire({
                    title: 'Số điểm của bạn có thể bị thay đổi bạn có chắc chắn sửa  ?',
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Đồng ý',
                    confirmButtonColor: '#ed5565',
                    cancelButtonText: 'Thoát'
                }).then((result) => {
                    if (result.value) {
                        let value = parseInt(event.floatValue)
                        formik.setFieldValue("checkChangePoi", true)
                        if (value > (customerPoint + customerPointOld)){
                            formik.setFieldValue("point", customerPoint + customerPointOld)
                            toastr.error(`Bạn chỉ có ${customerPoint + customerPointOld} không thể nhập nhiều hơn`)
                            return
                        }
                        if(value < 0){
                            formik.setFieldValue("point", 0)
                            toastr.error(`Bạn không thể nhập số nhỏ hơn 0`)
                            return
                        }
                        formik.setFieldValue("point", value)

                    } else if (result.dismiss === Swal.DismissReason.cancel) {
                        return false;
                    }
                })
            }else{
                let value = parseInt(event.floatValue)
                formik.setFieldValue("checkChangePoi", true)
                if (value > (customerPoint + customerPointOld)){
                    formik.setFieldValue("point", customerPoint + customerPointOld)
                    toastr.error(`Bạn chỉ có ${customerPoint + customerPointOld} không thể nhập nhiều hơn`)
                    return
                }
                if(value < 0){
                    formik.setFieldValue("point", 0)
                    toastr.error(`Bạn không thể nhập số nhỏ hơn 0`)
                    return
                }
                formik.setFieldValue("point", value)
            }
        }else{
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
        , applyPointIncrease, applyPointDecrease, applyPoint, handShowModalCoupon}, state: {customerPointOld, productTotalPrice, showModalCoupon, listCoupon, listProduct, productCartSelect, customerPoint, productSelect , showModelDetailProduct}};
}

export default MainController;