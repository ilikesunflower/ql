import React , {useState, useEffect}from 'react';
import {formatNumber} from '../../../../common/app';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {InputField, TextareaField} from "../../../../components/formikField"
import SelectNew from "../../../../components/SelectNew"
import {getListAddressProvince, getListAddressDistrict, getListAddressCommune} from "./httpService"

import NumberFormat from "react-number-format";
function AddressController(props) {
    let {formik} = props;
    
    const [provinceCodes, setProvinceCodes] = useState([]);
    const [districtCodes, setDistrictCodes] = useState([]);
    const [communeCodes, setCommuneCodes] = useState([]); 
    useEffect(function () {
        getListAddressProvince(function (rs) {
            setProvinceCodes(rs);
        })
    }, [])

    useEffect(function () {
        if( formik.values.provinceCode != ''){
            getListAddressDistrict({code: formik.values.provinceCode }, function (rs) {
                setDistrictCodes(rs);
                if(Array.isArray(rs) && rs.length > 0){
                    let check = rs.findIndex(x => x.code == formik.values.districtCode);
                    if(check == -1){
                        setCommuneCodes([]);
                        formik.setFieldValue("districtCode", '');
                        formik.setFieldValue("communeCode", '');
                    }
                }
            })
        }
    }, [formik.values.provinceCode])

    useEffect(function () {
        if( formik.values.districtCode != ''){
            getListAddressCommune({code: formik.values.districtCode }, function (rs) {
                setCommuneCodes(rs);
            })
        }
    }, [formik.values.districtCode])
    return {
        method:{},
        state: {
            provinceCodes,
            districtCodes,
            communeCodes
        }}
}
function Address(props) {
    const {method, state} = AddressController(props);
    let {formik} = props;
    return (
        <>
            <Card>
                <Card.Header>
                    <span className="card-title namePageText2 ">Địa chỉ giao hàng</span>
                </Card.Header>
                <Card.Body className="row">
                    <div className="form-check col-12 ml-2">
                         <input type="checkbox"  className="form-check-input" id="exampleCheck1" {...formik.getFieldProps("addressType")}
                                value="1" checked={formik.values.addressType == "1"}/>
                         <label className="form-check-label" htmlFor="exampleCheck1">Đặt hàng hộ <br/> (Giao tới địa chỉ người nhận khác)</label>
                    </div>
                    <Form.Group className="col-md-4 pt-3">
                        <Form.Label className="form-check-label">Họ tên  <span className="text-danger">(*)</span> </Form.Label>
                        <InputField className="form-control-xl form-control " placeholder="Nhập tên"  formik={formik} name="name"/>
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-3">
                        <Form.Label className="form-check-label">Số điện thoại <span className="text-danger">*</span> </Form.Label>
                        <InputField className="form-control-xl form-control " placeholder="Nhập số điện thoại"  formik={formik} name="phone"/>
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-3">
                        <Form.Label className="form-check-label">Email <span className="text-danger">*</span> </Form.Label>
                        <InputField className="form-control-xl form-control " placeholder="Nhập email"  formik={formik} name="email"/>
                    </Form.Group>
                    <Form.Group className="col-md-12 pt-3">
                        <Form.Label className="form-check-label">Địa chỉ nhận hàng <span className="text-danger">*</span> </Form.Label>
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-1">
                        <SelectNew options={state.provinceCodes} defaultValue={formik.values.provinceCode} formik={formik} name="provinceCode"   selectKey="code" selectText="name" placeholder=" Tỉnh / Thành phố"  />
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-1">
                        <SelectNew options={state.districtCodes} defaultValue={formik.values.districtCode} formik={formik} name="districtCode"   selectKey="code" selectText="name" placeholder=" Quận / Huyện "  />
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-1">
                        <SelectNew options={state.communeCodes} defaultValue={formik.values.communeCode} formik={formik} name="communeCode"   selectKey="code" selectText="name" placeholder=" Phường / Xã "  />
                    </Form.Group>
                    <Form.Group className="col-md-12 pt-1">
                        <Form.Label className="form-check-label"> </Form.Label>
                        <InputField placeholder="Địa chỉ chi tiết (Số nhà, đường phố...)" name="address"
                                    formik={formik}/>
                    </Form.Group>
                </Card.Body>
                <Card.Footer>
                    <Form.Group >
                        <Form.Label className="form-check-label">Ghi chú đặt hàng </Form.Label>
                        <TextareaField className="form-control" id="note" name="note" rows="3"
                                       placeholder="Nhập nội dung" formik={formik}/>
                    </Form.Group>
                </Card.Footer>
            </Card>
        </>
    )
}

export default Address;