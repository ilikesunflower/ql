import React , {useState, useEffect}from 'react';
import {formatNumber} from '../../../common/app';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {InputField, TextareaField} from "../../../components/formikField"
import SelectNew from "../../../components/SelectNew"
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
                    <span className="card-title namePageText2 ">?????a ch??? giao h??ng</span>
                </Card.Header>
                <Card.Body className="row">
                    <div className="form-check col-12 ml-2">
                         <input type="checkbox"  className="form-check-input" id="exampleCheck1" {...formik.getFieldProps("addressType")}
                                value="1" checked={formik.values.addressType == "1"}/>
                         <label className="form-check-label" htmlFor="exampleCheck1">?????t h??ng h??? <br/> (Giao t???i ?????a ch??? ng?????i nh???n kh??c)</label>
                    </div>
                    <Form.Group className="col-md-4 pt-3">
                        <Form.Label className="form-check-label">H??? t??n  <span className="text-danger">(*)</span> </Form.Label>
                        <InputField className="form-control-xl form-control " placeholder="Nh???p t??n"  formik={formik} name="name"/>
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-3">
                        <Form.Label className="form-check-label">S??? ??i???n tho???i <span className="text-danger">*</span> </Form.Label>
                        <InputField className="form-control-xl form-control " placeholder="Nh???p s??? ??i???n tho???i"  formik={formik} name="phone"/>
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-3">
                        <Form.Label className="form-check-label">Email <span className="text-danger">*</span> </Form.Label>
                        <InputField className="form-control-xl form-control " placeholder="Nh???p email"  formik={formik} name="email"/>
                    </Form.Group>
                    <Form.Group className="col-md-12 pt-3">
                        <Form.Label className="form-check-label">?????a ch??? nh???n h??ng <span className="text-danger">*</span> </Form.Label>
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-1">
                        <SelectNew options={state.provinceCodes} defaultValue={formik.values.provinceCode} formik={formik} name="provinceCode"   selectKey="code" selectText="name" placeholder=" T???nh / Th??nh ph???"  />
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-1">
                        <SelectNew options={state.districtCodes} defaultValue={formik.values.districtCode} formik={formik} name="districtCode"   selectKey="code" selectText="name" placeholder=" Qu???n / Huy???n "  />
                    </Form.Group>
                    <Form.Group className="col-md-4 pt-1">
                        <SelectNew options={state.communeCodes} defaultValue={formik.values.communeCode} formik={formik} name="communeCode"   selectKey="code" selectText="name" placeholder=" Ph?????ng / X?? "  />
                    </Form.Group>
                    <Form.Group className="col-md-12 pt-1">
                        <Form.Label className="form-check-label"> </Form.Label>
                        <InputField placeholder="?????a ch??? chi ti???t (S??? nh??, ???????ng ph???...)" name="address"
                                    formik={formik}/>
                    </Form.Group>
                </Card.Body>
                <Card.Footer>
                    <Form.Group >
                        <Form.Label className="form-check-label">Ghi ch?? ?????t h??ng </Form.Label>
                        <TextareaField className="form-control" id="note" name="note" rows="3"
                                       placeholder="Nh???p n???i dung" formik={formik}/>
                    </Form.Group>
                </Card.Footer>
            </Card>
        </>
    )
}

export default Address;