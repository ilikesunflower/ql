import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {InputField, NumberFormatField, NumberFormatFieldAfter} from "../../../components/formikField";
import NumberFormat from "react-number-format";
import PropertyController from "./PropertyController";

function PropertyView(props) {
    let {formik,  listProperties, listProperProduct} = props;
    const {state, method} = PropertyController(props);
    let meta = formik.getFieldMeta("checkExitSku") ;
    let meta1 = formik.getFieldMeta("codeStock")
    const withValueLimit = ({ floatValue }) => floatValue <= 999999999;
    console.log(listProperties);
    return(
        <>
            <Form.Group className="col-md-12 pt-3">
                <Card className="">
                    <Card.Header style={{fontSize:"18px"}}>
                        Thông tin bán hàng
                    </Card.Header>
                    <Card.Body>
                        <div className="text-center">
                            {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
                            {meta1.touched && meta1.error ? (<span className="text-danger">{meta1.error}</span>) : null}
                        </div>
                        <div className="row  d-flex justify-content-center">
                            {
                                (listProperties.length > 0)
                                && <Properties deleteT={method.deleteProperties} deleteDetailProperties={method.deleteDetailProperties} listProperty={listProperties}  addDetailProperties={method.addDetailProperties}  listProperties={state.listProperties} hand11={method.handFormProperties11} hand1={method.handFormProperties1}/>
                            }


                            {(listProperties.length < 3) && (
                                <div className="col-12 text-center">
                                    <Row className="d-flex justify-content-center">
                                        <div className="col-8">
                                            <Card>
                                                <Button variant="danger" className="btn btnButtonY" onClick={method.addFormProperties} >
                                                    <i className="far fa-plus pr-1"></i>
                                                    Thêm thuộc tính
                                                </Button>
                                            </Card>
                                        </div>
                                    </Row>

                                </div>
                            )}
                            {
                                (listProperties.length == 0)
                                && (
                                    <div className="col-12 pb-3"  >
                                        <div className="col-12 pb-3" >
                                            <div className="row pb-3"  >
                                                <Form.Label className="col-md-2  ">Tồn kho <span className="text-danger">*</span> </Form.Label>
                                                <NumberFormatField className="form-control-xl form-control col-md-8 "   formik={formik} name="quantityStock" />

                                            </div>
                                            <div className="row pb-3" >
                                                <Form.Label className="col-md-2  ">Mã kho hàng <span className="text-danger">*</span></Form.Label>
                                                <InputField err={false} className="form-control-xl form-control col-md-8  pl-2 "   formik={formik} name="codeStock" />

                                            </div>
                                            <div className="row">
                                                <Form.Label className=" col-md-2 "> Giá bán <span className="text-danger">*</span></Form.Label>
                                                <NumberFormatFieldAfter className="form-control-xl form-control  col-md-12 "   classnamediv="price_class col-8 pl-0"   placeholder="Nhập giá bán thị trường "  formik={formik} name="price" />
                                            </div>
                                        </div>
                                    </div>
                                )
                            }
                            {listProperProduct.length > 0 && (
                                <div className="col-12 pb-3"  >
                                    <div className="row">
                                        <Form.Label className="col-12  " style={{fontSize:"18px"}}>Danh sách phân loại hàng </Form.Label>

                                        <Col md={12}>
                                            <div className="table-responsive">
                                                <Table className=" table-bordered " >
                                                    <thead>
                                                    <tr>
                                                        <th >Tên</th>
                                                        <th style={{width: "20%"}}>Mã kho hàng</th>
                                                        <th style={{width: "20%"}}>Giá (đồng)</th>
                                                        <th style={{width: "20%"}}>Tồn kho</th>
                                                    </tr>
                                                    </thead>
                                                    <tbody>
                                                    {listProperProduct.map((item, i)=> {
                                                        return (
                                                            <tr key={i}>
                                                                <td className="">{item.name}</td>
                                                                <td><input defaultValue={item.skuMh} type='text'  onBlur={(e) => method.handSkuMh(e, i)}  className="form-control-xl form-control " name="skuMh"/></td>
                                                                <td>
                                                                    <NumberFormat  className="form-control form-control-xl  "  thousandSeparator={'.'} decimalSeparator={','} name="price" autoComplete="off"
                                                                                   value={!Number.isNaN(Number.parseInt(item.price)) ? Number.parseInt(item.price) : 0} onValueChange={(e) => method.handPrice(e,i)}/>
                                                                </td>
                                                                <td>
                                                                    <NumberFormat  isAllowed={withValueLimit}  className="form-control form-control-xl  " thousandSeparator={'.'} decimalSeparator={','} name="price" autoComplete="off"
                                                                                   value={!Number.isNaN(Number.parseInt(item.quantity)) ? Number.parseInt(item.quantity) : 0} onValueChange={(e) => method.handQuantitySkuMh(e,i)}/>
                                                                </td>
                                                            </tr>
                                                        )
                                                    })}

                                                    </tbody>
                                                </Table>
                                            </div>
                                        </Col>
                                    </div>
                                </div>
                            )}

                        </div>
                    </Card.Body>
                </Card>
            </Form.Group>
          
        </>
    );
}

const Properties = props => {
    const {hand11, hand1, listProperty, deleteT , addDetailProperties, deleteDetailProperties} = props;
    console.log(listProperty);
    return(
        <div className="col-12 pb-3"  >
            {listProperty.map((value, index)  => {
                return(
                    <div className="col-12 pb-3" key={value?.ord} >
                        <div className="row" >
                            <Form.Label className="col-2  ">Nhóm phân loại {index + 1} </Form.Label>
                            <div  className="col-8" >
                                <Card>
                                    <i className="fas fa-minus-circle p-1 buttonDelete" onClick={() => deleteT(value)}></i>
                                    <Card.Body className="row">
                                        <Form.Group className="col-md-12 pt-3">
                                            <div className="row">
                                                <span className="col-md-2  ">Tên nhóm phân loại </span>
                                                <div className="col-md-10">
                                                    <div className="row" >
                                                        <div className="col-11">
                                                            <input  defaultValue={value?.name} onBlur={(e) => hand1(e, value)}  className="form-control-xl form-control" placeholder="Nhập tên phân loại, Ví dụ size ..." size="xl" name="isHot"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </Form.Group>
                                        <div className="col-12">
                                            <div className="row">
                                                <div className="col-md-2 pt-3">
                                                    <span >Phân loại hàng </span>
                                                </div>
                                                <div className="col-md-10 ">
                                                    {(value != null && value?.properties.length > 0 ) && (
                                                        value?.properties.map((x )=> {
                                                            return(
                                                                <div className="row" key={x?.ord}>
                                                                    <div className="col-11">
                                                                        <input defaultValue={x?.value}  onBlur={(e) => hand11(e, value, x)}  placeholder="Nhập phân loại, Ví dụ S,M" className="form-control-xl form-control mt-3 " size="xl" name="ssd"/>
                                                                    </div>
                                                                    <div className="col-1">
                                                                        {(value?.properties.length != 1) && (<i className="far fa-trash-alt iconTrash pt-4" onClick={() =>deleteDetailProperties(value, x)}></i>)}
                                                                    </div>
            
                                                                </div>
                                                            )
                                                        })
                                                    )}
                                                </div>
            
                                                <div className="col-md-12 pt-3  d-flex justify-content-center">
                                                    <Button  variant="danger"  className="btn btnButtonY" onClick={(e) => addDetailProperties(e, value)}  >
                                                        <i className="far fa-plus"></i>
                                                        Thêm phân loại hàng
                                                    </Button>
                                                </div>
                                            </div>
                                        </div>
            
            
            
                                    </Card.Body>
                                </Card>
                            </div>
                        </div>
                    </div>
                )
            })}
        </div>

    );

}
export default PropertyView;