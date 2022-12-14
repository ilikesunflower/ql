import React, {useEffect, useState} from 'react';
import {isAllowedNumberIntThan0AndMax, isAllowedNumberThan0} from '../../common/app';
import {changeOrderConfirm,changeOrderShip,changeOrderSuccess,changeOrderCancel, statusPayment, changeOrderSynchronized, getReasonOrderCancel} from './service/httpService';
import {InputField, TextareaField,NumberFormatField,SelectField} from "../../components/formikField";
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {useFormik} from "formik";
import Yup from "../../components/Yup";
import SelectNew from "../../components/SelectNew";



function MainView(props){
    const group = $("#groupStatusBtn");
    const orderCode =group.attr("data-codeOrder").toLowerCase();
    const isShowAll = group.attr("data-isStatusShowAll") ?? "";
    const isOrderConfirm = group.attr("data-isOrderConfirm") ?? "";
    const isStatusPayment = group.attr("data-isStatusPayment") ?? "";
    const isOrderShip = group.attr("data-isOrderShip") ?? "";
    const isOrderSuccess = group.attr("data-isOrderSuccess") ?? "";
    const isOrderCancel = group.attr("data-isOrderCancel") ?? "";
    const isOrderSynchronized = group.attr("data-synchronized") ?? "";
    const priceCOD = parseInt(group.attr("data-sumPrice") ?? "");
    const shipPartner = parseInt(group.attr("data-shipPartner") ?? "");
    const typepayment = parseInt(group.attr("data-typepayment") ?? -1);
    const orderConfirm = () => {
        Swal.fire({
            title: 'Bạn có chắc chắn xác nhận đơn hàng này?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            confirmButtonColor: '#ed5565',
            cancelButtonText: 'Thoát'
        }).then((result) => {
            if (result.value) {
                let param = {
                    id: orderCode,
                }
                changeOrderConfirm(param, function (rs) {
                    if ( rs.statusCode == 200 ){
                        window.location.reload();
                    }else{
                        toastr.error(rs.message,"Thông báo");
                    }
                })
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                return false;
            }
        });
    }
    const orderSynchronized = () => {
        Swal.fire({
            title: 'Bạn có chắc chắn đồng bộ đơn hàng này?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            confirmButtonColor: '#ed5565',
            cancelButtonText: 'Thoát'
        }).then((result) => {
            if (result.value) {
                let param = {
                    id: orderCode,
                }
                changeOrderSynchronized(param, function (rs) {
                    if ( rs.statusCode == 200 ){
                        window.location.reload();
                    }else{
                        toastr.error(rs.message,"Thông báo");
                    }
                })
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                return false;
            }
        });
    }
    const formikCodShip = useFormik({
        initialValues: {
            cod : '',
            show: false,
            maxCod : priceCOD
        },
        validationSchema: Yup.object().shape({
            cod: Yup.string().required("Vui lòng nhập số tiền cần thu hộ"),
        }),
        onSubmit: (values, {resetForm}) => {
            Swal.fire({
                title: 'Bạn có chắc chắn gửi đối tác vận chuyển đơn hàng này?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                confirmButtonColor: '#ed5565',
                cancelButtonText: 'Thoát'
            }).then((result) => {
                if (result.value) {
                    let param = {
                        id: orderCode,
                        cod:values.cod
                    }
                    changeOrderShip(param, function (rs) {
                        if ( rs.statusCode == 200 ){
                            window.location.reload();
                        }else{
                            toastr.error(rs.message,"Thông báo");
                        }
                    })
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    return false;
                }
            });
        }
    })
    
    const orderShip = () => {
        if ( (shipPartner == 1 || shipPartner == 2)){
            if ( priceCOD > 5000000 && typepayment == 0 ){
                formikCodShip.setFieldValue("show", true);
            }else{
                Swal.fire({
                    title: 'Bạn có chắc chắn gửi đối tác vận chuyển đơn hàng này?',
                    type: 'warning',
                    showCancelButton: true,
                    confirmButtonText: 'Đồng ý',
                    confirmButtonColor: '#ed5565',
                    cancelButtonText: 'Thoát'
                }).then((result) => {
                    if (result.value) {
                        let param = {
                            id: orderCode,
                            cod:priceCOD
                        }
                        changeOrderShip(param, function (rs) {
                            if ( rs.statusCode == 200 ){
                                window.location.reload();
                            }else{
                                toastr.error(rs.message,"Thông báo");
                            }
                        })
                    } else if (result.dismiss === Swal.DismissReason.cancel) {
                        return false;
                    }
                });
            }
        }else{
            Swal.fire({
                title: 'Bạn có chắc chắn gửi đối tác vận chuyển đơn hàng này?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                confirmButtonColor: '#ed5565',
                cancelButtonText: 'Thoát'
            }).then((result) => {
                if (result.value) {
                    let param = {
                        id: orderCode,
                        cod:priceCOD
                    }
                    changeOrderShip(param, function (rs) {
                        if ( rs.statusCode == 200 ){
                            window.location.reload();
                        }else{
                            toastr.error(rs.message,"Thông báo");
                        }
                    })
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    return false;
                }
            });
        }
    };
    
    const orderSuccess = () => {
        Swal.fire({
            title: 'Bạn có chắc chắn hoàn thành đơn hàng này?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            confirmButtonColor: '#ed5565',
            cancelButtonText: 'Thoát'
        }).then((result) => {
            if (result.value) {
                let param = {
                    id: orderCode,
                }
                changeOrderSuccess(param, function (rs) {
                    if ( rs.statusCode == 200 ){
                        window.location.reload();
                    }else{
                        toastr.error(rs.message,"Thông báo");
                    }
                })
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                return false;
            }
        });
    }
    
    const formikOrderCancel = useFormik({
        initialValues: {
            note : 0,
            show: false
        },
        validationSchema: Yup.object().shape({
            note: Yup.number().min(0,"Vui lòng chọn lý do hủy đơn "),
        }),
        onSubmit: (values, {resetForm}) => {
            Swal.fire({
                title: 'Bạn có chắc chắn hủy đơn hàng này?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                confirmButtonColor: '#ed5565',
                cancelButtonText: 'Thoát'
            }).then((result) => {
                if (result.value) {
                    let param = {
                        id: orderCode,
                        note: values.note
                    }
                    changeOrderCancel(param, function (rs) {
                        if ( rs.statusCode == 200 ){
                            window.location.reload();
                        }else{
                            toastr.error(rs.message,"Thông báo");
                        }
                    })
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    return false;
                }
            });
        }
    }) 
    
    const orderCancel = () => {
        formikOrderCancel.setFieldValue("show",true);
    }

    const formikStatusPayment = useFormik({
        initialValues: {
            Status : 1,
            show: false
        },
        validationSchema: Yup.object().shape({
        }),
        onSubmit: (values, {resetForm}) => {
            Swal.fire({
                title: 'Bạn có chắc chắn muốn xác nhận thanh toán đơn hàng này?',
                type: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Đồng ý',
                confirmButtonColor: '#ed5565',
                cancelButtonText: 'Thoát'
            }).then((result) => {
                if (result.value) {
                    let param = {
                        OrderCode: orderCode,
                        status: values.status
                    }
                    statusPayment(param, function (rs) {
                        if ( rs.statusCode == 200 ){
                            window.location.reload();
                        }else{
                            toastr.error(rs.message,"Thông báo");
                        }
                    })
                } else if (result.dismiss === Swal.DismissReason.cancel) {
                    return false;
                }
            });
        }
    })
    const statusPaymentBtn = () => {
        formikStatusPayment.setFieldValue("show",true);
    }
    
    return(<>
        { isOrderSynchronized == "True" && <button className="btn btn-secondary m-l-r-5" onClick={orderSynchronized}>Đồng bộ Kiot</button>}

        { isOrderConfirm == "True" && <button className="btn btn-warning m-l-r-5" onClick={orderConfirm}>Xác nhận đơn</button>}
        { isOrderShip == "True" &&
            <>
                <button className="btn btn-info m-l-r-5" onClick={orderShip}>Gửi vận chuyển</button>
                <FormInputCodMoneyView formik={formikCodShip}/>
            </>
        }
        { isOrderSuccess == "True" && <button className="btn btn-success m-l-r-5" onClick={orderSuccess}>Hoàn thành đơn</button>}
        { isStatusPayment == "True" &&
            <>
                <button className="btn btn-default m-l-r-5" onClick={statusPaymentBtn}>Xác nhận thanh toán</button>
                <FormInputStatusPaymentView formik={formikStatusPayment} />
            </>
        }
        { isOrderCancel == "True" &&
            <>
                <button className="btn btn-danger m-l-r-5" onClick={orderCancel}>Hủy đơn</button>
                <FormOrderCancelView formik={formikOrderCancel} />
            </>
        }
    </>)
}

function FormOrderCancelView(props) {
    const {formik} = props;
    let [listReason, setListReason] = useState([]);
    useEffect(function () {
            getReasonOrderCancel(function (rs) {

                setListReason(rs);
            })
    }, [])
    const cancel = () => {
        formik.setFieldValue("show", false);
    }
    return (<Modal  show={formik.values.show}  animation={false}>
        <Form className="form-horizontal" onSubmit={formik.handleSubmit}>
            <Modal.Header>
                <Modal.Title>Hủy đơn</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form.Group className="col-md-12">
                    <Form.Label className="form-check-label">Lý do hủy đơn <span className="text-danger">*</span></Form.Label>
                    <SelectNew options={listReason} defaultValue={formik.values.note} formik={formik} name="note"   selectKey="type" selectText="name"   />
                </Form.Group>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" type="submit">
                    Lưu
                </Button>
                <Button variant="default" type="button" onClick={cancel}>
                    Thoát
                </Button>
            </Modal.Footer>
        </Form>
    </Modal>);
}

function FormInputCodMoneyView(props) {
    const {formik} = props;
    const cancel = () => {
        formik.setFieldValue("show", false);
    }
    return (<Modal  show={formik.values.show}  animation={false}>
        <Form className="form-horizontal" onSubmit={formik.handleSubmit}>
            <Modal.Header>
                <Modal.Title>Gửi vận chuyển</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form.Group className="col-md-12">
                    <Form.Label className="form-check-label">Số tiền thu hộ <span className="text-danger">*</span></Form.Label>
                    <NumberFormatField className="form-control-xl form-control " isAllowed={(values) => { return isAllowedNumberIntThan0AndMax(values,formik.values.maxCod) }} formik={formik} name="cod"/>
                </Form.Group>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" type="submit">
                    Lưu
                </Button>
                <Button variant="default" type="button" onClick={cancel}>
                    Thoát
                </Button>
            </Modal.Footer>
        </Form>
    </Modal>);
}
function FormInputStatusPaymentView(props) {
    const {formik} = props;
    const cancel = () => {
        formik.setFieldValue("show", false);
    }
    const option = [{key: "0", value: "Chưa thanh toán"}, {key: "1",value: "Đã thanh toán"}]
    return (<Modal  show={formik.values.show}  animation={false}>
        <Form className="form-horizontal" onSubmit={formik.handleSubmit}>
            <Modal.Header>
                <Modal.Title>Xác nhận thanh toán đơn hàng</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                <Form.Group className="col-md-12">
                    <SelectField className="form-control" formik={formik} options={option} name="status"></SelectField>
                </Form.Group>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="primary" type="submit">
                    Lưu
                </Button>
                <Button variant="default" type="button" onClick={cancel}>
                    Thoát
                </Button>
            </Modal.Footer>
        </Form>
    </Modal>);
}

export default MainView;