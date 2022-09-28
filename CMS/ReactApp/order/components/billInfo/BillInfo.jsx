import React from 'react';
import {Card, Form} from "react-bootstrap";
import {InputField} from "../../../components/formikField"

function BillInfo(props) {
    let {customerSelect, formik} = props;
    return (
        <>
            <Card>
                <Card.Header>
                    <span className="card-title namePageText2 ">  {customerSelect?.typeGroup == 2 ? "Thông tin đính kèm" : "Thông tin xuất hóa đơn"} </span>
                </Card.Header>
                <Card.Body className="row">
                    {
                        customerSelect?.typeGroup == 2 ?
                            <>
                                <Form.Group className="col-md-6 pt-3">
                                    <Form.Label className="form-check-label">Số PR <span
                                        className="text-danger">*</span> </Form.Label>
                                    <InputField className="form-control-xl form-control " placeholder="Nhập PR..."
                                                formik={formik} name="prCode"/>
                                </Form.Group>
                                <Form.Group className="col-md-6 pt-3">
                                    <Form.Label className="form-check-label">File PO đính kèm <span
                                        className="text-danger">*</span> </Form.Label><br/>
                                    <InputField className="form-control-xl form-control" name="prFile" formik={formik}
                                                placeholder="Nhập link file PO."/>
                                </Form.Group>
                            </>
                            :
                            <>
                                <Form.Group className="col-md-6 pt-3">
                                    <Form.Label className="form-check-label">Tên công ty </Form.Label>
                                    <InputField className="form-control-xl form-control " placeholder="Nhập tên công ty"
                                                formik={formik} name="billCompanyName"/>
                                </Form.Group>
                                <Form.Group className="col-md-6 pt-3">
                                    <Form.Label className="form-check-label">Địa chỉ </Form.Label>
                                    <InputField className="form-control-xl form-control " placeholder="Nhập địa chỉ"
                                                formik={formik} name="billAddress"/>
                                </Form.Group>
                                <Form.Group className="col-md-6 pt-3">
                                    <Form.Label className="form-check-label">Mã số thuế</Form.Label>
                                    <InputField className="form-control-xl form-control " placeholder="Nhập mã số thuế"
                                                formik={formik} name="billTaxCode"/>
                                </Form.Group>
                                <Form.Group className="col-md-6 pt-3">
                                    <Form.Label className="form-check-label">Email nhận hóa đơn</Form.Label>
                                    <InputField type="email" className="form-control-xl form-control "
                                                placeholder="Nhập email" formik={formik} name="billEmail"/>
                                </Form.Group>
                            </>
                    }

                </Card.Body>

            </Card>
        </>
    )
}

export default BillInfo;