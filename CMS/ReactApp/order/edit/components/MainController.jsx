import React , {useEffect, useState, useMemo} from 'react';
import Yup from "../../../components/Yup"
import {useFormik} from "formik";
import {isHtml, parStr2Float, isPoi} from "../../../common/app"
import {
    getListProduct,
    getProductCartList,
    checkGetPointCustomer,
    checkCouponCustomer,
    getListCustomer,
    getAddressCustomerDefault,
    checkShipmentCost,
    editOrder,
    getOrderEdit,
    getCustomer, getPriceProduct, getListOrderEdit, getListCustomerCoupon, getPointOldCustomer
} from "../service/httpService";
import validMessage from "../../../helpers/ValidMessage"

function MainController(props) {
    const {id} = props;
    let [customer, setCustomer] = useState(0);
    let [customerSelect, setCustomerSelect] = useState(null)
    const formik = useFormik({
        initialValues: {
            customerId: 0,
            orderId: id,
            checkChangeP: false,
            priceShip: 0,
            couponDiscount: 0,
            couponCode: '',
            point: 0,
            checkChangePoi: false,
            addressType: 0,
            customerAddressId: '',
            provinceCode: '',
            districtCode: '',
            communeCode: '',
            address: '',
            name: '',
            email: '',
            phone: '',
            note: '',
            shipPartner: '0',
            shipType: '0',
            paymentType: '0',
            billCompanyName: '',
            billAddress: '',
            billTaxCode: '',
            billEmail: '',
            prCode: '',
            prFile: '',
            total : 0,
            totalWeight: 0

        },
        validationSchema: Yup.object().shape({
            name: Yup.string().required("Vui lòng nhập họ tên").validHtml().maxLength(255),
            phone: Yup.string().required("Vui lòng nhập số điện thoại").validHtml().maxLength(15).min(10, "Vui lòng nhập ít nhất 10 kí tự"),
            email: Yup.string().email('Vui lòng nhập đúng định dạng email').required("Vui lòng nhập email").validHtml().maxLength(255),
            provinceCode: Yup.string().required("Vui lòng chọn tỉnh"),
            districtCode: Yup.string().required("Vui lòng chọn huyện"),
            communeCode: Yup.string().required( "Vui lòng chọn xã"),
            prCode: Yup.string().min(4, validMessage.min(4)).max(20, validMessage.max(20)).test('required', validMessage.required, (value) => {
                return (customerSelect?.type === 2 && value != null )|| (customerSelect?.type ===  1);
            } ),
            prFile: Yup.mixed().test('required', validMessage.required, (value) => {
                return (customerSelect?.type === 2 && value != null) || (customerSelect?.type ===  1)
            }),
            shipType: Yup.string().when("shipPartner", {
                is:(field) =>
                    {
                        return  Number.parseInt(field) !== 0 &&  Number.parseInt(field) != 3
                    },
                then: Yup.string().required(validMessage.required).test('required',validMessage.required, (value) => (value !== '0' && value !== '3'))
            }),
        }),
        onSubmit: async (values, {resetForm}) => {
            Swal.fire({
                title: 'Bạn muốn thay đổi đơn hàng ?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                confirmButtonColor: '#ed5565',
                cancelButtonText: 'Thoát'
            }).then((result) => {
                if (result.value) {
                    saveOrder()
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    return false;
                }
            })
        },
    });
   let [listProduct, setListProduct] = useState([]);
   let [listProductSelect, setListProductSelect] = useState([]);
   let [showModelDetailProduct, setShowModelDetailProduct] = useState(false);
   let [productSelect, setProductSelect] = useState(0);
   let [productCartSelect, setProductCartSelect] = useState([]);
    let [discountCode,setDiscountCode] = useState('');
    let [customerPoint,setCustomerPoint] = useState(0);
    let [customerPointOld,setCustomerPointOLd] = useState(0);
    let [checkDeleteProduct, setCheckDeleteProduct] = useState(true)
    let [point, setPoint] = useState(0);
    let [listCoupon, setListCoupon] = useState([]);
    let [showModalCoupon,setShowModalCoupon] = useState(false);

 
    const handShowModalCoupon = () => {
        setShowModalCoupon(!showModalCoupon);
    }
    const [shipmentPartners, setShipmentPartners] = useState([{
        name: "Nhận hàng tại kho",
        shipmentTypes: [],
        type: 0
    },
        {
            name : "Đối tác khác",
            shipmentTypes : [],
            type : 3
        }]);
    useEffect(function () {
        if(listProductSelect.length > 0 && !checkDeleteProduct){
            let count = listProductSelect.length - 1;
            let param = {
                Id : listProductSelect[count].productSimilarId,
                QuantityBy : listProductSelect[count].quantity,
                Order: count,
                Price:  Number.parseInt(listProductSelect[count].price)
            }
            getProductCartList(param, function (rs) {
                let data = [...productCartSelect, rs];
                setProductCartSelect(data);
            })
        }else if( listProductSelect.length == 0){
            setProductCartSelect([]);
        }
    },[listProductSelect] )
 
    useEffect(function () {
        if(id != 0){
            getOrderEdit({id: id}, function (rs) {
                formik.setFieldValue("customerId", rs?.customerId || 0)
                formik.setFieldValue("priceShip", rs?.priceShip || 0)
                formik.setFieldValue("couponCode", rs?.couponCode || '')
                formik.setFieldValue("couponDiscount", rs?.couponDiscount || 0)
                formik.setFieldValue("point", rs?.point || 0)
                formik.setFieldValue("addressType", rs?.addressType || 0)
                formik.setFieldValue("provinceCode", rs.orderAddress?.provinceCode || '' )
                formik.setFieldValue("districtCode", rs.orderAddress?.districtCode || '' )
                formik.setFieldValue("communeCode", rs.orderAddress?.communeCode || '' )
                formik.setFieldValue("address", rs.orderAddress?.address || '' )
                formik.setFieldValue("name", rs.orderAddress?.name || '' )
                formik.setFieldValue("email", rs.orderAddress?.email || '' )
                formik.setFieldValue("phone", rs.orderAddress?.phone || '' )
                formik.setFieldValue("note", rs?.note || '' )
                formik.setFieldValue("shipPartner", rs?.shipPartner || 0 )
                formik.setFieldValue("shipType", rs?.shipType || 0 )
                formik.setFieldValue("paymentType", rs?.paymentType || 0 )
                formik.setFieldValue("billCompanyName", rs?.billCompanyName || '' )
                formik.setFieldValue("billAddress", rs?.billAddress || '' )
                formik.setFieldValue("billTaxCode", rs?.billTaxCode || '' )
                formik.setFieldValue("billEmail", rs?.billEmail || '' )
                formik.setFieldValue("billCompanyName", rs?.billCompanyName || '' )
                formik.setFieldValue("prCode", rs?.prCode || '' )
                formik.setFieldValue("prFile", rs?.prFile || null )
                formik.setFieldValue("totalWeight", rs?.totalWeight || 0 )
                setPoint(rs?.point)
                if(Array.isArray(rs.orderProduct) && rs.orderProduct.length > 0){
                  let productS =    rs.orderProduct.map(x =>  {
                        return {
                            productId :  x.productId,
                            productSimilarId :  x.productSimilarId,
                            quantity :  x.quantity,
                            price : x.price
                        };
                    })
                    setListProductSelect(productS);
 
                };
                getCustomer({id: rs?.customerId}, function (rs) {
                    setCustomerSelect(rs) ;
                    if(rs?.type == 2 ){
                        formik.setFieldValue("paymentType", 3 )
                    }
                    setCustomer(rs?.id || 0)
                });
                getListOrderEdit({id: id}, function (rs) {
                    setProductCartSelect(rs);
                })
                checkGetPointCustomer({customerId : rs?.customerId},function (response) {
                    setCustomerPoint(response || 0)
                })
                getListCustomerCoupon({customerId : rs?.customerId, orderId :id },function (response) {
                   setListCoupon(response);
                })
            })
            getPointOldCustomer({orderId: id}, function (rs) {
                console.log(rs);
                setCustomerPointOLd(rs);
            })
            getListProduct(function (rs) {
                setListProduct(rs);
            })
        }
    }, [])
    const priceShip = formik.values?.priceShip;
    let couponDiscount = formik.values.couponDiscount
    let couponCode = formik.values.couponCode

    const productTotalPrice = useMemo(() => {
        return productCartSelect.reduce(function (previousValue, currentValue) {
            return previousValue + (currentValue?.price || 0) * (currentValue?.quantityBy || 0);
        } ,0)
    }, [productCartSelect])

    let totalPrice =useMemo(function () {
        let total = productTotalPrice + priceShip - formik.values.point * isPoi - couponDiscount;
        if (total < 0){
            total = 0;
        }
        formik.setFieldValue("total", total);
        return total;
    },[productTotalPrice,priceShip,formik.values.point,couponDiscount])
    
    const getShipCost = async function () {
        let { provinceCode, districtCode, communeCode} = formik.values;

        if (productCartSelect.length === 0) {
            return;
        }
        let shipmentCost = [{
            name: "Nhận hàng tại kho",
            shipmentTypes: [],
            type: 0
        },
            {
                name : "Đối tác khác",
                shipmentTypes : [],
                type : 3
            }];
        let weight = productCartSelect.reduce((previousValue, currentValue) => previousValue + (currentValue.weight * currentValue.quantityBy|| 0), 1);
        formik.setFieldValue("totalWeight", weight)
        if ( provinceCode && districtCode && communeCode) {
            let param = {provinceCode, districtCode, communeCode, weight};
            checkShipmentCost(param , function (rs) {
                shipmentCost = rs?.shipmentPartners || [];
                setShipmentPartners(shipmentCost);
            })
        }
        setShipmentPartners(shipmentCost);
    }
    const saveOrder = () => {

        const formData = new FormData()

        for (const key in formik.values) {
            if ( !Array.isArray(formik.values[key]) ){
                formData.append(key,formik.values[key]);
            }
        }
        productCartSelect.forEach( (product, i ) => {
            formData.append(`products[${i}][productSimilarId]`,product.productSimilarId);
            formData.append(`products[${i}][quantity]`,product.quantityBy);
            formData.append(`products[${i}][price]`,product.price);
            formData.append(`products[${i}][weight]`,product.weight || 0);
        } )
        editOrder(formData, function (rs) {
            if (rs.code === 200) {
                window.location.href = "/Orders/Order/Details?id="+ rs.content;
            } else {
                toastr.error("Sửa đơn hàng thất bại. Vui lòng thử lại !")

            }

        })
    }
    useEffect(function () {
            getShipCost()
    }, [
        productCartSelect,
        formik.values.provinceCode,
        formik.values.districtCode,
        formik.values.communeCode,
    ])

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
   const clickQuantityBuy = (type, index) => {
       let data = [...productCartSelect];
       let check = data[index];
        getPriceProduct({id : check.productSimilarId}, function (rs) {
            if(  rs?.price !== check.price ||  rs?.weight !== check?.weight ){
                Swal.fire({
                    title: 'Giá đã thay đổi bạn có muốn thay đổi số lượng không ?',
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Đồng ý',
                    confirmButtonColor: '#ed5565',
                    cancelButtonText: 'Thoát'
                }).then((result) => {
                    if (result.value) {
                        let quantityBuy = check.quantityBy;
                        let quantityWH = check.quantityWH;
                        data[index].price = rs?.price || 0;
                        data[index].weight = rs?.weight || 0;
                        if ( type == 1 ){
                            let q = quantityBuy - 1;
                            if ( q < 1 ){
                                data[index].quantityBy = 1;
                            }else{
                                data[index].quantityBy = q;
                            }
                        }else if(type == 2){
                            let q = quantityBuy + 1;
                            if ( q > (quantityWH + quantityBuy) ){
                                data[index].quantityBy = quantityWH + quantityBuy;
                            }else{
                                data[index].quantityBy = q;
                            }
                        }
                        setProductCartSelect(data);
                    } else if (result.dismiss === Swal.DismissReason.cancel) {
                        return false;
                    }
                })
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
                    if ( q > ( quantityWH + quantityBuy) ){
                        data[index].quantityBy = quantityWH + quantityBuy;
                    }else{
                        data[index].quantityBy = q;
                    }
                }
                setProductCartSelect(data);
            }

        });
     
      
    }
   const changeQuantityBuy = (e, index) => {
           let quantity = Math.floor(e.floatValue);
           let data = [...productCartSelect];
           let check = data[index];
       getPriceProduct({id : check.productSimilarId}, function (rs) {
           if( rs?.price !== check.price ||  rs?.weight !== check?.weight){
               Swal.fire({
                   title: 'Giá đã thay đổi bạn có muốn thay đổi số lượng không ?',
                   type: 'warning',
                   showCancelButton: true,
                   confirmButtonText: 'Đồng ý',
                   confirmButtonColor: '#ed5565',
                   cancelButtonText: 'Thoát'
               }).then((result) => {
                   if (result.value) {
                       let quantityBuy = check.quantityBy;
                       let quantityWH = check.quantityWH;
                       data[index].price =  rs?.price || 0;
                       data[index].weight = rs?.weight || 0;
                       if ( quantity > (quantityWH + quantityBuy) ){
                           data[index].quantityBy =  quantityWH + quantityBuy;
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
           }else{
               let quantityBuy = check.quantityBy;
               let quantityWH = check.quantityWH;
               if ( quantity > (quantityWH + quantityBuy) ){
                   data[index].quantityBy = quantityWH + quantityBuy;
               }else if ( quantity >= 1 && quantity <= (quantityWH + quantityBuy)){
                   data[index].quantityBy = quantity;

               }else{
                   data[index].quantityBy = 1;
               }
               setProductCartSelect(data);
           }

       });
         

   }
   const deleteProductSelect = (index) => {
        let data = [...productCartSelect];
        data.splice(index, 1);
        setProductCartSelect(data)
        setCheckDeleteProduct(true);
       let data1 = [...listProductSelect];
       data1.splice(index, 1);
       setListProductSelect(data1)
    }
    const applyPointDecrease = () => {
        if(point > (customerPoint + customerPointOld) &&  !formik.values.checkChangePoi ) {
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
      
    }
    const applyPointIncrease = () => {
       if(point > (customerPoint + customerPointOld) &&  !formik.values.checkChangePoi ){
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
       
       
    }
    const applyPoint = (event) => {
        if(point > (customerPoint + customerPointOld) &&  !formik.values.checkChangePoi ){
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
    }
    const applyDiscountCode =  () => {
        if (!discountCode){
            formik.setFieldValue("couponDiscount",0)
            formik.setFieldValue("couponCode",  '')
            toastr.error("Vui lòng nhập mã giảm giá!")
            return;
        }
        checkCouponCustomer({code : discountCode, customerId : customer}, function (response) {
            if (response.code === 200){
                let data = response.content;
                formik.setFieldValue("couponDiscount", data?.reducedPrice)
                formik.setFieldValue("couponCode",  data?.code)
                return;
            }
            formik.setFieldValue("couponDiscount",0)
            formik.setFieldValue("couponCode",  '')
            if (response.code === 400){
                toastr.error("Có lỗi trong quá trình áp dụng mã!")
                return;
            }
            if (response.code === 404){
                toastr.error("Mã giảm giá không hợp lệ")
            }
        })
  
        
    }
    return {
       formik,
        state:{
            listProduct,
            productSelect,
            showModelDetailProduct,
            listProductSelect,
            productCartSelect,productTotalPrice,
            priceShip,
            point,
            couponCode,
            totalPrice,
            customerPoint,
            discountCode,
            couponDiscount,
            customer,
            shipmentPartners,
            customerSelect,
            listCoupon, showModalCoupon,
            customerPointOld
        },
        method:{
            handleChangeSelect,
            handShowDetailProduct,
            setListProductSelect,
            setShowModelDetailProduct,
            changeQuantityBuy, 
            clickQuantityBuy,
            deleteProductSelect,
            applyPoint, applyPointIncrease, applyPointDecrease,
            setDiscountCode,
            applyDiscountCode,
            setCheckDeleteProduct,
            handShowModalCoupon
        } };
  
}
export default MainController;