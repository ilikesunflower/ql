import React from 'react';
import {formatNumber, isPoi} from '../../../common/app';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";

import NumberFormat from "react-number-format";
import Select2ComponentNew from "../../../components/Select2ComponentNew";
import MainController from "./MainController";
import DetailProduct from "../detailProduct/MainView";
import Coupon from "../modelCoupon/Coupon";
import {NumberFormatField} from  "../../../components/formikField"

function MainView(props) {
    let {formik, isEdit} = props;
    let {state,method } = MainController(props) ;
    return (
        <>
            {(state.showModelDetailProduct) && <DetailProduct formik={formik} showModelDetailProduct={state.showModelDetailProduct} productCartSelect={state.productCartSelect} setProductCartSelect={method.setProductCartSelect}  id={state.productSelect} handShowDetailProduct={method.handShowDetailProduct}/>}
            {(state.showModalCoupon) && <Coupon showModalCoupon={state.showModalCoupon} handShowModalCoupon={method.handShowModalCoupon} listCoupon={state.listCoupon} formik={formik} />}

            <Card>
                <Form >
                    <Card.Header >
                        <span className="card-title namePageText2 ">Thông tin sản phẩm</span>
                    </Card.Header>
                    <Card.Body className="row">
                        <Form.Group className="col-md-12 pt-3">
                            <Form.Label className="form-check-label">Chọn sản phẩm  <span className="text-danger">*</span> </Form.Label>
                            <div className="input-group row  m-0">
                                <Select2ComponentNew defaultValue={state.productSelect} onChange={method.handleChangeSelect} name="productSelect" className=" rounded-0 col-md-10 col-sm-9 " placeholder="Chọn sản phẩm" options={state.listProduct} selectKey={"id"} selectText={"name"} />
                                <span className="input-group-append  col-md-2 col-sm-3   pr-0">
                                     <button type="button" className="btn btn-default btn-xl buttonFont col-12 " onClick={method.handShowDetailProduct} >
                                           <i className="far fa-plus" ></i>
                                     </button>
                                </span>
                            </div>
                        </Form.Group>
                        {
                            (state.productCartSelect.length > 0) &&
                            <>
                                <div className="table-responsive  col-12 mt-3 pl-3">
                                    <table className="table table-bordered ">
                                        <thead>
                                        <tr>
                                            <th>Sản phẩm</th>
                                            <th className="text-center d-none d-xl-table-cell">Đơn giá (đồng)</th>
                                            <th className="text-center d-none d-xl-table-cell">Số lượng</th>
                                            <th className="text-center d-none d-xl-table-cell">Thành tiền (đồng)</th>
                                            <th className="text-center d-none d-xl-table-cell">Thao tác</th>
                                        </tr>
                                        </thead>
                                        <tbody>
                                        {(state.productCartSelect.length > 0) &&
                                            state.productCartSelect.map((item, i) => {
                                                return(
                                                    <tr key={i}>
                                                        <td className="  m-0  align-middle" >
                                                            <div className="row">
                                                                <img src={item.image} className="imageCart col-md-2"/>
                                                                <div className=" col-md-10">
                                                                    {item.nameProduct}<br/>
                                                                    {(item.listProperties.length > 0) && item.listProperties.map((item1 , k) => {
                                                                        return(
                                                                            <span  key={k}>
                                                                                            <span >{item1.propertiesName} : {item1.propertiesValueName}</span><br/>
                        
                                                                                        </span>
                                                                        )
                                                                    })}
                                                                </div>
                                                            </div>
                        
                                                        </td>
                                                        <td className="text-center  align-middle">
                                                            {formatNumber(item.price)}
                                                        </td>
                                                        <td className="text-center  align-middle">
                                                            <div className="btn-group ">
                                                                <button type="button" className="btn " onClick={(e) => method.clickQuantityBuy(1, i)}>
                                                                    <i className="fa-solid fa-minus"></i>
                                                                </button>
                                                                <NumberFormat  className="form-control form-control-xl inputQuantity  " thousandSeparator={'.'} decimalSeparator={','} name="price" autoComplete="off"
                                                                               value={!Number.isNaN(Number.parseInt(item.quantityBy)) ? Number.parseInt(item.quantityBy) : 0}  onValueChange={e =>  {
                                                                    method.changeQuantityBuy(e, i)}}/>
                                                                <button type="button"  className="btn " onClick={(e) => method.clickQuantityBuy(2, i)}>
                                                                    <i className="fa-solid fa-plus"></i>
                                                                </button>
                                                            </div><br/>
                                                            <span className="textCss">(Còn {formatNumber(item.quantityWH)} sản phẩm)</span>
                                                        </td>
                                                        <td className="text-center  align-middle">
                                                            {formatNumber(item.quantityBy * item.price)}
                                                        </td>
                                                        <td className="text-center  align-middle">
                                                            <button type="button" className="buttonNoColor" onClick={() => method.deleteProductSelect(i)} ><i className="fa-solid fa-trash text-hover-red"></i></button>
                                                        </td>
                                                    </tr>
                                                )
                                            })
                                        }
                                        </tbody>
                                    </table>
                                </div>
                                <div className=" col-12 mt-3 pl-3">
                                    <div className="row">
                                        <div className="col-12">
                                            <div className="row mt-3 ">
                                                <div className="col-md-10">
                                                    <div className="d-flex justify-content-end">
                                                        <strong>Tạm tính</strong>
                                                    </div>
                                                </div>
                                                <div className="col-md-2">
                                                    <div className="d-flex justify-content-end">
                                                        <strong>{formatNumber(state.productTotalPrice)} <u>đ</u></strong>
                                                    </div>
                                
                                                </div>
                                            </div>
                                        </div>
                                        <div className="col-12">
                                            <div className="row mt-3">
                                                <div className="col-md-10">
                                                    <div className="d-flex justify-content-end">
                                                        <strong>Phí giao hàng { (formik.values?.priceNoSale != 0 && <>({formatNumber(formik.values?.priceNoSale )} | -{formik.values?.percent}%)</> )}</strong>
                                                    </div>
                                                </div>
                                                {isEdit ?
                                                    <div className="col-md-2">
                                                        <div className="d-flex justify-content-end">
                                                            <NumberFormatField formik={formik} name="priceShip"
                                                                               className="form-control col-md-8 text-right"/>
                                                        </div>

                                                    </div> :
                                                    <div className="col-md-2">
                                                        <div className="d-flex justify-content-end">
                                                            <strong>{formatNumber(formik.values?.priceShip )} <u>đ</u></strong>
                                                        </div>

                                                    </div>
                                                }
                                                
                                               
                                            </div>
                                        </div>
                                        {/*<div className="col-12">*/}
                                        {/*    <div className="row mt-3">*/}
                                        {/*        <div className="col-md-10">*/}
                                        {/*            <div className="styleLastTable justify-content-end">*/}
                                        {/*                <span className="text-right">Sử dụng điểm thưởng</span>*/}
                                        {/*                <span className="txt-helper">( Bạn đang có {formatNumber(isEdit? state.customerPoint + state.customerPointOld :state.customerPoint)} điểm)</span>*/}
                                        {/*                <div className="btn-group ">*/}
                                        {/*                    <button type="button" className="btn " onClick={method.applyPointDecrease}>*/}
                                        {/*                        <i className="fa-solid fa-minus"></i>*/}
                                        {/*                    </button>*/}
                                        {/*                    <NumberFormat  className="form-control form-control-xl inputQuantity  " thousandSeparator={'.'} decimalSeparator={','} name="price" autoComplete="off"*/}
                                        {/*                                   value={!Number.isNaN(Number.parseInt(formik.values.point)) ? Number.parseInt(formik.values.point) : 0} onValueChange={method.applyPoint}/>*/}
                                        {/*                    <button type="button"  className="btn " onClick={method.applyPointIncrease}>*/}
                                        {/*                        <i className="fa-solid fa-plus"></i>*/}
                                        {/*                    </button>*/}
                                        {/*                </div>*/}
                                        {/*            </div>*/}
                                        
                                        {/*        </div>*/}
                                        {/*        <div className="col-md-2">*/}
                                        {/*            <div className="d-flex justify-content-end">*/}
                                        {/*                <strong>-{formatNumber(formik.values?.point * isPoi)} <u>đ</u></strong>*/}
                                        {/*            </div>*/}
                                        
                                        {/*        </div>*/}
                                        {/*    </div>*/}
                                        {/*</div>*/}
                                        <div className="col-12">
                                            <div className="row mt-3">
                                                <div className="col-md-10">
                                                    <div className="styleLastTable justify-content-end">
                                                        <span className="text-right">Coupon { !formik.values?.couponCode ? '' : `(${formik.values?.couponCode})` }</span>
                                                        <span className="txt-helper">( Sử dụng coupon để được giá tốt nhất )</span>
                                                        <div className="d-flex justify-content-end">
                                                            <div className="input-group input-group-sm w-193px">
                                                                <input type="text" value={formik.values.couponCode} disabled={true}  className="form-control form-control-sm"
                                                                       placeholder="Nhập mã"/>
                                                                <span className="input-group-append">
                                                                    <button type="button" className="btn btn-danger btn-flat"  onClick={method.handShowModalCoupon}><i className="fa-solid fa-plus"></i></button>
                                                                           
                                                                </span>
                                                            </div>
                                                        </div>
                                        
                                                    </div>
                                        
                                                </div>
                                                <div className="col-md-2">
                                                    <div className="d-flex justify-content-end">
                                                        <strong>-{formatNumber(formik.values.couponDiscount )} <u>đ</u></strong>
                                                    </div>
                                        
                                                </div>
                                            </div>
                                        </div>
                                        <div className="col-12">
                                            <div className="row mt-3">
                                                <div className="col-md-10">
                                                    <div className="styleLastTable justify-content-end">
                                                        <strong>Tổng thanh toán</strong>
                                                        <span className="txt-helper">( Đã bao gồm VAT)</span>
                                                    </div>
                                                </div>
                                                <div className="col-md-2">
                                                    <div className="d-flex justify-content-end">
                                                        <strong className="total-price">{formatNumber(formik.values?.total)} <u>đ</u></strong>
                                                    </div>
                                        
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </>
                        }
                    </Card.Body>
                </Form>
            </Card>
        </>
    )
}

export default MainView;