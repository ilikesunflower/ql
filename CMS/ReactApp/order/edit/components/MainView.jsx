import React , {useEffect, useState, useMemo} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import MainController from "./MainController";
import Address from "../../components/address/Address"
import Shipment from "../../components/shipment/Shipment"
import Payment from "../../components/payment/Payment"
import BillInfo from "../../components/billInfo/BillInfo"

import ListProduct from "../../components/listproduct/MainView";
function MainApp(props) {
    
    const {state, method, formik} = MainController(props);
    console.log("formik", formik.values)
    return (
        <Row>
          <Col md={12}>
                <Card>
                    <Form >
                        <Card.Header >
                            <span className="card-title namePageText2 ">Sửa đơn hàng</span>
                        </Card.Header>
                        <Card.Body  className="row">
                            <Form.Group className="col-md-12 pt-3">
                                <Form.Label className="form-check-label">Khách hàng  </Form.Label> 
                                <input className="form-control" value={state.customerSelect != null ? state.customerSelect?.fullName : ""} disabled={true}/>
                            </Form.Group>
                          
                            
                        </Card.Body>
                    </Form>
                </Card>
              {
                  state.customer != 0  &&  <ListProduct  productCartSelect={state.productCartSelect} setProductCartSelect={method.setProductCartSelect}
                                                         formik={formik}  customer={state.customer}   isEdit={true} orderId={state.orderId}
                  />
              }
               
           
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