import React , {useEffect, useState, useMemo} from 'react';
import Yup from "../../../components/Yup"
import {useFormik} from "formik";
import {isHtml, parStr2Float, isPoi} from "../../../common/app"
import {
    editOrder,
    getOrderEdit,
    getCustomer
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
                return (customerSelect?.typeGroup == 2 && value != null )|| (customerSelect?.typeGroup !=  2);
            } ),
            prFile: Yup.mixed().test('required', validMessage.required, (value) => {
                return (customerSelect?.typeGroup == 2 && value != null) || (customerSelect?.typeGroup !=  2)
            }),
            shipType: Yup.string().when("shipPartner", {
                is:(field) =>
                    {
                        return  Number.parseInt(field) != 0 &&  Number.parseInt(field) != 3 
                    },
                then: Yup.string().required(validMessage.required).test('required',validMessage.required, (value) => (value !== '0' && value !== '3' ))
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
   let [productCartSelect, setProductCartSelect] = useState([]);

    useEffect(function () {
        if(id != 0){
            getOrderEdit({id: id}, function (rs) {
                console.log("get data rs", rs)
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
                formik.setFieldValue("note", rs?.orderAddress?.note || '' )
                formik.setFieldValue("shipPartner", (!rs?.shipPartner ||  rs?.shipPartner == 2) ? 0 : rs?.shipPartner )
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
                getCustomer({id: rs?.customerId}, function (rs) {
                    setCustomerSelect(rs) ;
                    if(rs?.typeGroup == 2 ){
                        formik.setFieldValue("paymentType", 3 )
                    }
                    setCustomer(rs?.id || 0)
                });
            })
        }
    }, [])
  
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


    
    return {
       formik,
        state:{
            customer,
            customerSelect,
            orderId:id,
            productCartSelect
        },
        method:{
            setProductCartSelect
        } };
  
}
export default MainController;