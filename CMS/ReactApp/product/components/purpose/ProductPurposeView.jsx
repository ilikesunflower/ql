import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {CheckBoxField, InputField} from '../../components/formikField'
import ProductPurposeController from "./ProductPurposeController"

function ProductPurposeView(props) {
    
    let {formik, state, method} = ProductPurposeController(props);
    return (
        <>
            <Modal  show={state.showPurpose}  onHide={method.handPurpose} animation={false}>
                <Form className="form-horizontal" onSubmit={formik.handleSubmit}>
                    <Modal.Header>
                        <Modal.Title>Thêm mục đích sử dụng</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Form.Group className="col-md-12">
                            <Form.Label className="form-check-label">Tên</Form.Label>
                            <InputField className="form-control-xl form-control " formik={formik} name="name"/>
                        </Form.Group>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={method.handPurpose}>
                            Hủy
                        </Button>
                        <Button variant="primary" type="submit">
                            Lưu
                        </Button>
                    </Modal.Footer>
                </Form>
            </Modal> 
            <Modal  show={state.showDeletePurpose}  onHide={method.handDeletePurpose} animation={false}>
                <Form className="form-horizontal" onSubmit={method.deletePurpose}>
                    <Modal.Header>
                        <Modal.Title>Xóa mục đích sử dụng</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <div className="row">
                            {(state.listProductPurpose.length > 0) && (
                                state.listProductPurpose.map((item, i) => {
                                    return(
                                        <Form.Group className="col-12 pt-3" key={i}>
                                            <div className="row">
                                                <input size="xl" type="checkbox"  onChange={() => method.clickElement(item.value)}   className=" col-3 pr-1"  />
                                                <span className="col-9 pr-2 ">{item.label} </span>
                                            </div>
                                        </Form.Group> 
                                    )
                                })
                            )}
                        </div>

                        <div>
                        </div>
                    </Modal.Body>
                    <Modal.Footer>
                        <Button variant="secondary" onClick={method.handDelete}>
                            Hủy  
                        </Button>
                        <Button variant="primary"  type="button" onClick={method.deletePurpose}>
                            Lưu
                        </Button>
                    </Modal.Footer>
                </Form>
            </Modal> 
        </>
    );
}


export default ProductPurposeView;