import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import "../../style.css" 
import MainController from "./MainController";
import ProductPurposeView from "../../components/purpose/ProductPurposeView";
import CategoryView from "../../components/category/CategoryView";
import PropertyView from "../../components/property/PropertyView";

import {
    InputField,
} from "../../../components/formikField"
import CropImageView from "../../components/cropImage/CropImageView";
import RightComponentView from "../../components/rightCompnent/RightComponentView";
import TabComponentView from "../../components/tabComponent/TabComponentView";

function MainApp(props) {
    const { state, method, formikProduct } = MainController();
    return (
        <Row>
            <Col md={12}>
                <Card>
                    <Form onSubmit={formikProduct.handleSubmit} >
                        <Card.Header className="row m-0 p-0 border-bottom-0 headerColor">
                            <div className="col-12">
                                <span className="card-title namePageText2 pl-3 pt-3">Thêm mới sản phẩm</span>
                                <div className="float-right pt-3  pr-3">
                                    <Button className="btn-success" type="submit">
                                        Lưu
                                    </Button>
                                </div>
                            </div>
                            <div className="col-12">
                                <ul className="nav nav-tabs" role="tablist">
                                    <li className="nav-item active">
                                        <a className="nav-link active" data-toggle="tab" href="#tab-1" aria-expanded="True">Thông tin cơ bản</a>
                                    </li>
                                    <li className="nav-item">
                                        <a className="nav-link" data-toggle="tab" href="#tab-2" aria-expanded="false"> Mô tả chi tiết</a>
                                    </li>
                                    <li className="nav-item">
                                        <a className="nav-link" data-toggle="tab" href="#tab-3" aria-expanded="false"> Thông số kĩ thuật</a>
                                    </li>
                                </ul>
                            </div>
                        </Card.Header>
                        <Card.Body >
                            <div className="tab-content row">
                                <div id="tab-1" className="tab-pane active col-12">
                                    <div className="row">
                                           <div className="col-xs-12 col-md-8 col-lg-9">
                                               <Card>
                                                   <Card.Body className="row">
                                                       <Form.Group className="col-md-6 pt-3">
                                                           <Form.Label className="form-check-label">Mã hàng  <span className="text-danger">*</span> </Form.Label>
                                                           <InputField className="form-control-xl form-control " placeholder="Nhập mã hàng"  formik={formikProduct} name="sku"/>
                                                       </Form.Group>
                                                       <Form.Group className="col-md-6 pt-3">
                                                           <Form.Label className="form-check-label">Tên mặt hàng  <span className="text-danger">*</span> </Form.Label>
                                                           <InputField className="form-control-xl form-control " placeholder="Nhập tên mặt hàng"  formik={formikProduct} name="name"/>
                                                       </Form.Group>
                                                        <ProductPurposeView formikProduct={formikProduct}/>
                                                        <CategoryView formikProduct={formikProduct}/>
                                                        <CropImageView isEdit={false} listFileSave={state.listFileSave}  setListFileSave={method.setListFileSave}/>
                                                    
                                                       <PropertyView formik={formikProduct} listProperties={state.listProperties} setListProperties={method.setListProperties}  listProperProduct={state.listProperProduct} setListProperProduct={method.setListProperProduct}/>
                                                   
                                                   </Card.Body>
                                               </Card>
                                           </div> 
                                        <RightComponentView formik={formikProduct} imageString={state.imageString}  setImageString={method.setImageString} />
                                    </div>
                                </div>
                                <TabComponentView formik={formikProduct}/>
                            
                            </div>
                        </Card.Body>
                    </Form>
                </Card>
            </Col>
        </Row>
    );
}


export default MainApp;