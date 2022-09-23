import React , {useEffect, useState, useMemo} from 'react';
import Yup from "../../../components/Yup"
import {useFormik} from "formik";
import {isHtml, parStr2Float, isPoi} from "../../../common/app"
import {
    createOrder,
} from "../service/httpService";
import validMessage from "../../../helpers/ValidMessage"

function MainController(props) {
    let [customer, setCustomer] = useState(0);
    let [customerSelect, setCustomerSelect] = useState(null)
    let [productCartSelect, setProductCartSelect] = useState([]);
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
    return {
       formik,
        state:{
            productCartSelect,
            customer,
            customerSelect,
        },
        method:{
            setProductCartSelect,
            setCustomer,
            setCustomerSelect
        } };
  
}
export default MainController;