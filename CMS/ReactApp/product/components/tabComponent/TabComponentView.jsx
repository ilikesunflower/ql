import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {
    Textarea, TextareaField
} from "../../../components/formikField";

function TabComponentView(props) {
    let {formik} = props;
   
    return(
        <>
            <div id="tab-2" className="tab-pane  col-12">
                <Card>
                    <Card.Body>
                        <Form.Group className="col-md-12 pt-3">
                            <div className="row">
                                <Form.Label className=" col-md-2 pr-2  " style={{fontSize:"18px"}}>Mô tả ngắn</Form.Label>
                                <TextareaField className="col-md-10"  placeholder="Nhập mô tả sản phẩm"   formik={formik} name="lead"/>
                            </div>
                        </Form.Group>
                        <Form.Group className="col-md-12 pt-3">
                            <div className="row">
                                <Form.Label className=" col-md-2 pr-2  " style={{fontSize:"18px"}}>Mô tả</Form.Label>
                                <Textarea className="col-md-10" placeholder="Nhập mô tả sản phẩm"   formik={formik} name="description"/>
                            </div>
                        </Form.Group>

                    </Card.Body>
                </Card>
            </div>
            <div id="tab-3" className="tab-pane  col-12">
                <Card>
                    <Card.Body>
                        <Form.Group className="col-md-12 pt-3">
                            <div className="row">
                                <Form.Label className=" col-md-2 pr-2  " style={{fontSize:"18px"}}>Thông số kĩ thuật</Form.Label>
                                <Textarea className="col-md-10"  placeholder="Nhập thông số kĩ thuật"   formik={formik} name="specifications"/>
                            </div>
                        </Form.Group>
                    </Card.Body>
                </Card>
            </div>
        </>
    );
}

export default TabComponentView;