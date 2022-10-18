import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {
    CheckBoxField,
    FileFieldCropImage,
    InputField,
    NumberFormatField,
    NumberFormatFieldAfter
} from "../../../components/formikField";
import SelectNew from "../../../components/SelectNew";
const optionsSex = [
    {value: 0 , label: "Tất cả"},
    {value: 1 , label: "Nam"},
    {value: 2, label: "Nữ"}
]
function RightComponentView(props) {
    let {formik, setImageString, imageString} = props;
   
    return(
        <>
            <div className="col-md-4 col-lg-3">
                <Card className="file-group">
                    <Card.Header>
                        <h5 >Ảnh chính <span className="text-danger">*</span> </h5>
                    </Card.Header>
                    <Card.Body>
                        <div className="row">

                            {/*<div className="col-lg-12 " onClick={()=> {$(state.refI.current).click()}}>*/}
                            <FileFieldCropImage widthStyle={300}  imageString={imageString} setImageString={setImageString} formik={formik}   name="image" className="hidden"/>
                            {/*<img src={state.imageString} className="imgA"/>*/}
                            {/*</div>*/}
                        </div>
                    </Card.Body>
                </Card>
                <Card>
                    <Card.Body className="row">
                        <Form.Group className="col-md-12 pt-3">
                            <Form.Label className="form-check-label">Cân nặng <span className="text-danger">*</span></Form.Label>
                            <NumberFormatFieldAfter className="form-control-xl form-control " classnamediv="weight_class"   placeholder="Nhập cân nặng"  formik={formik} name="weight" />
                        </Form.Group>
                        <Form.Group className="col-md-12 pt-3">
                            <Form.Label className="form-check-label">Giá gốc</Form.Label>
                            <NumberFormatFieldAfter className="form-control-xl form-control "  classnamediv="price_class"    placeholder="Nhập giá bán"  formik={formik} name="priceSale" />
                        </Form.Group>

                        <Form.Group className="col-md-12 pt-3">
                            <Form.Label className="form-check-label ">Giới tính   </Form.Label>
                            <SelectNew options={optionsSex} defaultValue={formik.values.productSex} formik={formik} name="productSex"   selectKey="value" selectText="label" placeholder="Chọn giới tính"  />
                        </Form.Group>
                        <Form.Group className="col-md-12 pt-3">
                            <Form.Label className="form-check-label">Độ tuổi </Form.Label>
                            <NumberFormatField className="form-control-xl form-control " formik={formik}  placeholder="Nhập độ tuổi"   name="productAge" />
                        </Form.Group>
                        <Form.Group className="col-md-12 pt-3">
                            <Form.Label className="form-check-label">Đơn vị  <span className="text-danger">*</span> </Form.Label>
                            <InputField className="form-control-xl form-control " formik={formik}  placeholder="Nhập đơn vị"   name="unit"/>
                        </Form.Group>
                    </Card.Body>
                </Card>

                <Card>
                    <Card.Body className="row">

                        <Form.Group className="col-12 pt-3">
                            <div className="row">
                                <Form.Label className="form-check-label col-9 pr-2 ">Sản phẩm nổi bật </Form.Label>
                                <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik} name="isHot"/>
                            </div>
                        </Form.Group>
                        <Form.Group className="col-md-12 pt-3">
                            <div className="row">
                                <Form.Label className="form-check-label col-9 pr-2 ">Sản phẩm mới </Form.Label>
                                <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik} name="isNew"/>
                            </div>
                        </Form.Group>
                        <Form.Group className="col-md-12 pt-3">
                            <div className="row">
                                <Form.Label className="form-check-label col-9 pr-2 ">Sản phẩm bán chạy </Form.Label>
                                <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik} name="isBestSale"/>
                            </div>
                        </Form.Group>
                        <Form.Group className="col-md-12 pt-3">
                            <div className="row">
                                <Form.Label className="form-check-label col-9  pr-2 ">Sản phẩm khuyến mãi </Form.Label>
                                <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik} name="isPromotion"/>
                            </div>
                        </Form.Group>
                        <Form.Group className="col-md-12 pt-3">
                            <div className="row">
                                <Form.Label className="form-check-label col-9 pr-2  ">Kích hoạt</Form.Label>
                                <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik} name="isPublic"/>
                            </div>
                        </Form.Group>
                    </Card.Body>
                </Card>
            </div>
        </>
    );
}

export default RightComponentView;