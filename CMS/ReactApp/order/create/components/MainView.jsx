﻿import React , {useEffect, useState, useMemo} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import Select from 'react-select'
import MainController from "./MainController";
import Select2ComponentNew from "../../../components/Select2ComponentNew";
import MainView from "./detailProduct/MainView"
import Coupon from "./modelCoupon/Coupon"
import {formatNumber} from '../../../common/app';
import NumberFormat from "react-number-format";

import Customer from "../../components/customer/MainView"
import Address from "./address/Address"
import Shipment from "./shipment/Shipment"
import Payment from "./payment/Payment"
import BillInfo from "./billInfo/BillInfo"
import {isPoi} from "../../../common/app"
function MainApp(props) {
    const {state, method, formik} = MainController();
    return (
        <Row>
            {(state.showModelDetailProduct) && <MainView setCheckDeleteProduct={method.setCheckDeleteProduct} showModelDetailProduct={state.showModelDetailProduct} setShowModelDetailProduct={method.setShowModelDetailProduct} setListProductSelect={method.setListProductSelect} listProductSelect={state.listProductSelect} setListProductPropertiesSelect={method.setListProductSelect}  id={state.productSelect} handShowDetailProduct={method.handShowDetailProduct}/>}
            {(state.showModalCoupon) && <Coupon showModalCoupon={state.showModalCoupon} handShowModalCoupon={method.handShowModalCoupon} listCoupon={state.listCoupon} formik={formik} />}
            <Col md={12}>
               <Customer method={{
                   setCustomer: method.setCustomer,
                   setCustomerSelect: method.setCustomerSelect
               }} state={{
                   customer: state.customer
               }} formik={formik}/>
               <Card>
                    <Form >
                            <Card.Header >
                                <span className="card-title namePageText2 ">Thông tin sản phẩm</span>
                            </Card.Header>
                            <Card.Body className="row">
                                <Form.Group className="col-md-12 pt-3">
                                    <Form.Label className="form-check-label">Chọn sản phẩm  <span className="text-danger">*</span> </Form.Label>
                                    <div className="input-group row  m-0">
                                        <Select2ComponentNew defaultValue={state.productSelect} onChange={method.handleChangeSelect} name="productSelect" className=" rounded-0 col-md-10 col-sm-9 " placeholder="Chọn sản phẩm" options={state.listProduct} selectKey={"id"} selectText={"name"} />
                                        <span className="input-group-append  col-md-2 col-sm-3   pr-0">
                                                    <button type="button" className="btn btn-default btn-xl buttonFont col-12 " onClick={method.handShowDetailProduct} >
                                                        <i className="far fa-plus" ></i>
                                                    </button>
                                        </span>
                                    </div>
                                </Form.Group>
                                {
                                    (state.productCartSelect.length > 0) &&
                                    <>
                                        <div className="table-responsive  col-12 mt-3 pl-3">
                                            <table className="table table-bordered ">
                                                <thead>
                                                <tr>
                                                    <th>Sản phẩm</th>
                                                    <th className="text-center d-none d-xl-table-cell">Đơn giá (đồng)</th>
                                                    <th className="text-center d-none d-xl-table-cell">Số lượng</th>
                                                    <th className="text-center d-none d-xl-table-cell">Thành tiền (đồng)</th>
                                                    <th className="text-center d-none d-xl-table-cell">Thao tác</th>
                                                </tr>
                                                </thead>
                                                <tbody>
                                                {(state.productCartSelect.length > 0) &&
                                                    state.productCartSelect.map((item, i) => {
                                                        return(
                                                            <tr key={i}>
                                                                <td className="  m-0  align-middle" >
                                                                    <div className="row">
                                                                        <img src={item.image} className="imageCart col-md-2"/>
                                                                        <div className=" col-md-10">
                                                                            {item.nameProduct}<br/>
                                                                            {(item.listProperties.length > 0) && item.listProperties.map((item1 , k) => {
                                                                                return(
                                                                                    <span  key={k}>
                                                                                        <span >{item1.propertiesName} : {item1.propertiesValueName}</span><br/>
        
                                                                                    </span>
                                                                                )
                                                                            })}
                                                                        </div>
                                                                    </div>
    
                                                                </td>
                                                                <td className="text-center  align-middle">
                                                                    {formatNumber(item.price)}
                                                                </td>
                                                                <td className="text-center  align-middle">
                                                                    <div className="btn-group ">
                                                                        <button type="button" className="btn " onClick={(e) => method.clickQuantityBuy(1, i)}>
                                                                            <i className="fa-solid fa-minus"></i>
                                                                        </button>
                                                                        <NumberFormat  className="form-control form-control-xl inputQuantity  " thousandSeparator={'.'} decimalSeparator={','} name="price" autoComplete="off"
                                                                                       value={!Number.isNaN(Number.parseInt(item.quantityBy)) ? Number.parseInt(item.quantityBy) : 0} onValueChange={(e) =>  {
                                                                            method.changeQuantityBuy(e, i)}}/>
                                                                        <button type="button"  className="btn " onClick={(e) => method.clickQuantityBuy(2, i)}>
                                                                            <i className="fa-solid fa-plus"></i>
                                                                        </button>
                                                                    </div><br/>
                                                                    <span className="textCss">(Còn {formatNumber(item.quantityWH)} sản phẩm)</span>
                                                                </td>
                                                                <td className="text-center  align-middle">
                                                                    {formatNumber(item.quantityBy * item.price)}
                                                                </td>
                                                                <td className="text-center  align-middle">
                                                                    <button type="button" className="buttonNoColor" onClick={() => method.deleteProductSelect(i)} ><i className="fa-solid fa-trash text-hover-red"></i></button>
                                                                </td>
                                                            </tr>
                                                        )
                                                    })
                                                }
                                                </tbody>
                                            </table>
                                        </div>
                                        <div className=" col-12 mt-3 pl-3">
                                            <div className="row">
                                                <div className="col-12">
                                                    <div className="row mt-3 ">
                                                        <div className="col-md-10">
                                                            <div className="d-flex justify-content-end">
                                                                <strong>Tạm tính</strong>
                                                            </div>
                                                        </div>
                                                        <div className="col-md-2">
                                                            <div className="d-flex justify-content-end">
                                                                <strong>{formatNumber(state.productTotalPrice)} <u>đ</u></strong>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                                <div className="col-12">
                                                    <div className="row mt-3">
                                                        <div className="col-md-10">
                                                            <div className="d-flex justify-content-end">
                                                                <strong>Phí giao hàng</strong>
                                                            </div>
                                                        </div>
                                                        <div className="col-md-2">
                                                            <div className="d-flex justify-content-end">
                                                                <strong>{formatNumber(state.priceShip)} <u>đ</u></strong>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                                <div className="col-12">
                                                    <div className="row mt-3">
                                                        <div className="col-md-10">
                                                            <div className="styleLastTable justify-content-end">
                                                                <span className="text-right">Sử dụng điểm thưởng</span>
                                                                <span className="txt-helper">( Bạn đang có {formatNumber(state.customerPoint)} điểm)</span>
                                                                <div className="btn-group ">
                                                                    <button type="button" className="btn " onClick={method.applyPointDecrease}>
                                                                        <i className="fa-solid fa-minus"></i>
                                                                    </button>
                                                                    <NumberFormat  className="form-control form-control-xl inputQuantity  " thousandSeparator={'.'} decimalSeparator={','} name="price" autoComplete="off"
                                                                                   value={!Number.isNaN(Number.parseInt(state.point)) ? Number.parseInt(state.point) : 0} onValueChange={method.applyPoint}/>
                                                                    <button type="button"  className="btn " onClick={method.applyPointIncrease}>
                                                                        <i className="fa-solid fa-plus"></i>
                                                                    </button>
                                                                </div>
                                                            </div>

                                                        </div>
                                                        <div className="col-md-2">
                                                            <div className="d-flex justify-content-end">
                                                                <strong>-{formatNumber(state.point * isPoi)} <u>đ</u></strong>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                                <div className="col-12">
                                                    <div className="row mt-3">
                                                        <div className="col-md-10">
                                                            <div className="styleLastTable justify-content-end">
                                                                <span className="text-right">Coupon { !state.couponCode ? '' : `(${state.couponCode})` }</span>
                                                                <span className="txt-helper">( Sử dụng coupon để được giá tốt nhất )</span>
                                                                <div className="d-flex justify-content-end">
                                                                    <div className="input-group input-group-sm w-193px">
                                                                        <input type="text" value={formik.values.couponCode} disabled={true} onChange={ev => method.setDiscountCode(ev.target.value)} className="form-control form-control-sm"
                                                                               placeholder="Nhập mã"/>
                                                                        <span className="input-group-append">
                                                                <button type="button" className="btn btn-danger btn-flat"  onClick={method.handShowModalCoupon}><i className="fa-solid fa-plus"></i></button>
                                                                       
                                                            </span>
                                                                    </div>
                                                                </div>

                                                            </div>

                                                        </div>
                                                        <div className="col-md-2">
                                                            <div className="d-flex justify-content-end">
                                                                <strong>-{formatNumber(state.couponDiscount )} <u>đ</u></strong>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                                <div className="col-12">
                                                    <div className="row mt-3">
                                                        <div className="col-md-10">
                                                            <div className="styleLastTable justify-content-end">
                                                                <strong>Tổng thanh toán</strong>
                                                                <span className="txt-helper">( Đã bao gồm VAT)</span>
                                                            </div>
                                                        </div>
                                                        <div className="col-md-2">
                                                            <div className="d-flex justify-content-end">
                                                                <strong className="total-price">{formatNumber(state.totalPrice)} <u>đ</u></strong>
                                                            </div>

                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </>
                                }
                            </Card.Body>
                        </Form>
               </Card>
              
                <Address formik={formik} />
                <div className="row">
                    <div className="col-md-6">
                        <Shipment formik={formik} shipmentPartners={state.shipmentPartners}/>
                    </div>
                    <div className="col-md-6">
                        <Payment customerSelect={state.customerSelect} formik={formik} />
                    </div>
                </div>
                    <BillInfo customerSelect={state.customerSelect}  formik={formik} customer={state.customer}/>
                <div className="row pt-3 pb-5">
                    <div className="col-12 ">
                        <div className="d-flex justify-content-end">
                            <button type="button" className="btn btn-next btn-defaultt " onClick={formik.submitForm}>Lưu đơn hàng</button>
                        </div>
                    </div>
                </div>
            </Col>
        </Row>
    );
}
export default MainApp;