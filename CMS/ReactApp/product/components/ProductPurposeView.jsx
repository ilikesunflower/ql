import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {CheckBoxField, InputField} from '../../components/formikField'
import '../style.css'

function ProductPurposeView(props) {
    let {formik, showDelete, showCreate, handPurpose, listProductPurpose, handDelete, clickElement, deletePurpose} = props;
    return (
        <>
            <Modal  show={showCreate}  onHide={handPurpose} animation={false}>
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
                        <Button variant="secondary" onClick={handPurpose}>
                            Hủy
                        </Button>
                        <Button variant="primary" type="submit">
                            Lưu
                        </Button>
                    </Modal.Footer>
                </Form>
            </Modal> 
            <Modal  show={showDelete}  onHide={handDelete} animation={false}>
                <Form className="form-horizontal" onSubmit={deletePurpose}>
                    <Modal.Header>
                        <Modal.Title>Xóa mục đích sử dụng</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <div className="row">
                            {(listProductPurpose.length > 0) && (
                                listProductPurpose.map((item, i) => {
                                    return(
                                        <Form.Group className="col-12 pt-3" key={i}>
                                            <div className="row">
                                                <input size="xl" type="checkbox"  onChange={() => clickElement(item.value)}   className=" col-3 pr-1"  />
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
                        <Button variant="secondary" onClick={handDelete}>
                            Hủy  
                        </Button>
                        <Button variant="primary"  type="button" onClick={deletePurpose}>
                            Lưu
                        </Button>
                    </Modal.Footer>
                </Form>
            </Modal> 
        </>
    );
}


export default ProductPurposeView;