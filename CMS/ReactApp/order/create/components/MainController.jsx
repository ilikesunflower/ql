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
    createOrder,
    getListCustomerCoupon
} from "../service/httpService";
import validMessage from "../../../helpers/ValidMessage"

function MainController(props) {
    let [listCustomer, setListCustomer] = useState([]);
    let [customer, setCustomer] = useState(0);
    let [customerSelect, setCustomerSelect] = useState(null)
    let [checkDeleteProduct, setCheckDeleteProduct] = useState(false)
    let [listCoupon, setListCoupon] = useState([]);
    const formik = useFormik({
        initialValues: {
            customerId: 0,
            priceShip: 0,
            couponDiscount: 0,
            couponCode: '',
            point: 0,
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
                    is: (field) => (field != "0" ||field != "3"),
                    then: Yup.string().required(validMessage.required).test('required',validMessage.required, (value) =>( value != '0'  ||value !== "3"))
            }),
        }),
        onSubmit: async (values, {resetForm}) => {
            Swal.fire({
                title: 'Bạn muốn lưu đơn hàng ?',
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
    let [showModalCoupon,setShowModalCoupon] = useState(false);
    useEffect(function () {
        if(customer != 0){
            checkGetPointCustomer({customerId : customer},function (response) {
                setCustomerPoint(response || 0)
            })
            setDiscountCode('');
            getListCustomerCoupon({customerId : customer}, function (response) {
                setListCoupon(response);
            })
        }
       
    },[customer])
    const [shipmentPartners, setShipmentPartners] = useState([{
        name: "Nhận hàng tại kho",
        shipmentTypes: [],
        type: 0
    }, {
            name : "Đối tác khác",
            shipmentTypes : [],
            type : 3
        }]);
   
    useEffect(function () {
        getListProduct(function (rs) {
            setListProduct(rs);
        })
        getListCustomer(function (rs) {
            setListCustomer(rs);
        })
       
    }, []);
    useEffect(function () {
        if(listProductSelect.length > 0 && !checkDeleteProduct){
            let count = listProductSelect.length - 1;
            let param = {
                Id : listProductSelect[count].productSimilarId,
                QuantityBy : listProductSelect[count].quantity,
                Order: count
            }
            getProductCartList(param, function (rs) {
                let data = [...productCartSelect, rs];
                setProductCartSelect(data);
            })
        }else if( listProductSelect.length == 0) {
            setProductCartSelect([]);
        }
    },[listProductSelect] )
 
    useEffect(function () {
        if(customer != 0){
            getAddressCustomerDefault({customerId : customer}, function (rs) {
                formik.setFieldValue("provinceCode", rs?.provinceCode || '');
                formik.setFieldValue("districtCode", rs?.districtCode || '');
                formik.setFieldValue("communeCode", rs?.communeCode || '');
                formik.setFieldValue("address", rs?.address || '');
                formik.setFieldValue("name", rs?.name || '');
                formik.setFieldValue("email", rs?.email || '');
                formik.setFieldValue("phone", rs?.phone || '');
            })
        }
    }, [customer])
    const priceShip = formik.values?.priceShip;
    let point = formik.values.point
    let couponDiscount = formik.values.couponDiscount
    let couponCode = formik.values.couponCode

    const productTotalPrice = useMemo(() => {
        return productCartSelect.reduce(function (previousValue, currentValue) {
            return previousValue + (currentValue?.price || 0) * (currentValue?.quantityBy || 0);
        } ,0)
    }, [productCartSelect])

    let totalPrice =useMemo(function () {
        let total = productTotalPrice + priceShip - point * isPoi - couponDiscount;
        if (total < 0){
            total = 0;
        }
        formik.setFieldValue("total", total);
        return total;
    },[productTotalPrice,priceShip,point,couponDiscount])
    
    const getShipCost = async function () {
        let { provinceCode, districtCode, communeCode} = formik.values;

        if (productCartSelect.length === 0) {
            return;
        }
        let shipmentCost = [
            {
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
        } )
        createOrder(formData, function (rs) {
            if (rs.code === 200) {
                window.location.href = "/Orders/Order/Details?id="+ rs.content;
            } else {
                toastr.error("Lưu đơn hàng thất bại. Vui lòng thử lại !")

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
    useEffect(function () {
        let shipPartner = shipmentPartners.find(partner => partner.type == formik.values.shipPartner);
        if (shipPartner) {
            let shipmentType = shipPartner?.shipmentTypes.find(shipmentType => shipmentType.type == formik.values.shipType);
            formik.setFieldValue("priceShip", shipmentType?.cost || 0);
        }
    }, [
        shipmentPartners,
        formik.values.shipPartner,
        formik.values.shipType
    ])

 
    const handleChangeSelect = (event) => {
       setProductSelect(event.value)
    }; 
    const handleChangeSelectCustomer = (event) => {
       setCustomer(event.value)
        let selecC = listCustomer.find(x => x.id == event.value);
       setCustomerSelect(selecC ?? null);
       if(selecC?.type == 2 ){
           formik.setFieldValue("paymentType",3)
       }else{
           formik.setFieldValue("paymentType",0)

       }
        formik.setFieldValue("customerId",event.value)

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
        setCheckDeleteProduct(true);
        let data1 = [...listProductSelect];
        data1.splice(index, 1);
        setListProductSelect(data1);
   }
    const applyPointDecrease = () => {
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
    const applyPointIncrease = () => {
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
                toastr.error("Mã giảm giá không hợp lê")
            }
        })
  
        
    }
    const handShowModalCoupon = () => {
       setShowModalCoupon(!showModalCoupon);
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
            listCustomer,
            customer,
            shipmentPartners,
            customerSelect,
            listCoupon,  showModalCoupon
          
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
            handleChangeSelectCustomer,
            setCheckDeleteProduct,
            handShowModalCoupon
        } };
  
}
export default MainController;