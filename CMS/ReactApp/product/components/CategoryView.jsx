import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {CheckBoxField, InputField} from '../../components/formikField'

function CategoryView(props) {
    const {handCategory, formik, show} = props;
    return(
        <Modal  show={show}  onHide={handCategory} animation={false}>
            <Form className="form-horizontal" onSubmit={formik.handleSubmit}>
                <Modal.Header >
                    <Modal.Title>Thêm danh mục sản phẩm</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form.Group className="col-md-12">
                        <Form.Label className="form-check-label">Tên  <span className="text-danger">*</span> </Form.Label>
                        <InputField className="form-control-xl form-control " formik={formik} name="name"/>
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handCategory}>
                        Hủy
                    </Button>
                    <Button variant="primary" type="submit">
                        Lưu
                    </Button>
                </Modal.Footer>
            </Form>
        </Modal>
    );
}


export default CategoryView;