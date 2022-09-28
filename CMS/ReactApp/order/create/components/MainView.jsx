import React , {useEffect, useState, useMemo} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import Select from 'react-select'
import MainController from "./MainController";
import Select2ComponentNew from "../../../components/Select2ComponentNew";
import ListProduct from "../../components/listproduct/MainView"
import {formatNumber} from '../../../common/app';
import NumberFormat from "react-number-format";

import Customer from "../../components/customer/MainView"
import Address from "../../components/address/Address"
import Shipment from "../../components/shipment/Shipment"
import Payment from "../../components/payment/Payment"
import BillInfo from "../../components/billInfo/BillInfo"
function MainApp(props) {
    const {state, method, formik} = MainController();
    console.log(state.customerSelect)
    return (
        <Row>
            <Col md={12}>
               <Customer method={{
                   setCustomer: method.setCustomer,
                   setCustomerSelect: method.setCustomerSelect
               }} state={{
                   customer: state.customer
               }} formik={formik}/>
                <ListProduct  productCartSelect={state.productCartSelect} setProductCartSelect={method.setProductCartSelect}
                              formik={formik}  customer={state.customer}  isEdit={false}
                />
                <Address formik={formik} />
                <div className="row">
                    <div className="col-md-6">
                        <Shipment formik={formik} productCartSelect={state.productCartSelect}/>
                    </div>
                    <div className="col-md-6">
                        <Payment customerSelect={state.customerSelect} formik={formik} />
                    </div>
                </div>
                    <BillInfo customerSelect={state.customerSelect}  formik={formik} />
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