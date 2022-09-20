import React , {useEffect, useState}from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import Select2ComponentNew from "../../../components/Select2ComponentNew";
import {getListCustomer} from "./httpService";

function MainView(props) {
    const {method, state , formik } = props;
    let [listCustomer, setListCustomer] = useState([]);
    useEffect(() => {
        getListCustomer(function (rs) {
            console.log(rs);
            setListCustomer(rs);
        })
    }, [])
    const handleChangeSelectCustomer = (event) => {
        method.setCustomer(event.value)
        let selecC = listCustomer.find(x => x.id == event.value);
        method.setCustomerSelect(selecC ?? null);
        if(selecC?.type == 2 ){
            formik.setFieldValue("paymentType",3)
        }else{
            formik.setFieldValue("paymentType",0)

        }
        formik.setFieldValue("customerId",event.value)

    }; 
    return (
        <Card>
            <Form >
                <Card.Header >
                    <span className="card-title namePageText2 ">Tạo đơn hàng</span>
                </Card.Header>
                <Card.Body  className="row">
                    <Form.Group className="col-md-12 pt-3">
                        <Form.Label className="form-check-label">Chọn khách hàng  <span className="text-danger">*</span> </Form.Label>
                        <Select2ComponentNew defaultValue={state.customer} onChange={handleChangeSelectCustomer} name="customer" className=" rounded-0 col-md-12 col-sm-12 pr-0" options={listCustomer} selectKey={"id"} selectText={"fullName"} />
                    </Form.Group>


                </Card.Body>
            </Form>
        </Card>
    )
}

export default MainView;