import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {CheckBoxField, InputField} from '../../components/formikField'
import CategoryController from "./CategoryController"
import SelectNewMutile2 from "../../../components/SelectNewMutile2";

function CategoryView(props) {
    let {formikProduct} = props;
    const {state, formik, method} = CategoryController(props);
    return(
        <>
            <Form.Group className="col-md-12 pt-3">
                <Form.Label className="form-check-label ">Danh mục sản phẩm  <span className="text-danger">*</span> </Form.Label>
                <SelectNewMutile2 isDisabled="disabled" options={state.listProductCategory} formik={formikProduct} name="productCategory" hand={method.handCategory} defaultValue={[]} selectKey="value" selectText="label" placeholder="Chọn danh mục" className="basic-multi-select rounded-0 col-md-11 col-sm-9 pr-0" />
            </Form.Group>
            <Modal  show={state.showCategory}  onHide={method.handCategory} animation={false}>
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
                        <Button variant="secondary" onClick={method.handCategory}>
                            Hủy
                        </Button>
                        <Button variant="primary" type="submit">
                            Lưu
                        </Button>
                    </Modal.Footer>
                </Form>
            </Modal>
        </>
    );
}


export default CategoryView;