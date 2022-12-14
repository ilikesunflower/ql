import React , {useState, useEffect}from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";

function Payment(props) {
    let {formik, customerSelect} = props
    const paymentTypeProps =  formik.getFieldProps("paymentType")
    const paymentType =  paymentTypeProps.value
    return (
        <>
            <Card>
                <Card.Header>
                    <span className="card-title namePageText2 ">Phương thức thanh toán</span>
                </Card.Header>
                <Card.Body >
                   <div className="row">
                       {customerSelect?.typeGroup == 2 
                           ? 
                           <span className="col-12">
                               Tổng giá trị đơn hàng sẽ được Daiichi thanh toán từ ngân sách của phòng ban
                           </span>
                           :
                           <div className="col-12">
                               <div className="row">
                                   <div className="col-md-6  text-center   d-flex align-items-stretch flex-column ">
                                       <div className="card d-flex flex-fill borderR ">
                                           <input id="radio_pay1" {...paymentTypeProps} type="radio" hidden value="0" checked={"0" == paymentType}/>
                                           <label htmlFor="radio_pay1"  className="p-3" >
                                               <strong className="radio-icon__title">Thanh toán khi nhận hàng</strong>
                                               <i className="radio-icon"></i>
                                           </label>
                                       </div>
                                   </div>
                                   {/*<div className="col-md-3    text-center d-flex align-items-stretch flex-column  ">*/}
                                   {/*    <div className="card d-flex flex-fill borderR">*/}
                                   {/*        <input id="radio_pay2" {...paymentTypeProps} type="radio" hidden value="1" checked={"1" == paymentType}/>*/}
                                   {/*        <label htmlFor="radio_pay2" className="p-3"  >*/}
                                   {/*            <strong className="radio-icon__title">Thẻ tín dụng</strong>*/}
                                   {/*            <i className="radio-icon"></i>*/}
                                   {/*        </label>*/}
                                   {/*    </div>*/}
                                   {/*</div>*/}
                                   <div className="col-md-6    text-center d-flex align-items-stretch flex-column ">
                                       <div className="card d-flex flex-fill borderR">
                                           <input id="radio_pay3" {...paymentTypeProps} type="radio" hidden value="2" checked={"2" == paymentType}/>
                                           <label htmlFor="radio_pay3" className="p-3"  >
                                               <strong className="radio-icon__title">Chuyển khoản Ngân hàng</strong>
                                               <i className="radio-icon"></i>
                                           </label>
                                       </div>
                                   </div>
                               </div>
                               <hr/>
                               {( "2" == paymentType) && (<>
                                   <dl className="row">
                                       <dt className="col-md-4">Tên tài khoản:</dt>
                                       <dd className="col-md-8">Công ty CP Giải pháp Công nghệ Việt Nam</dd>
                                       <dt className="col-md-4">Nội dung</dt>
                                       <dd className="col-md-8">Nội dung chuyển khoản: “Thanh toán cho đơn hàng ”DLVN_XXXXXXXXXX
                                           Trong đó XXXXXXXXXX là mã đơn hàng thành công được gửi vào Email</dd>
                                       <table className="table table-bordered ">
                                           <thead>
                                           <tr>
                                               <th className="text-center">Ngân hàng</th>
                                               <th className="text-center ">Số tài khoản</th>
                                           </tr>
                                           </thead>
                                           <tbody>
                                           <tr>
                                               <td>Ngân hàng TMCP Đầu tư và Phát triển Việt Nam</td>
                                               <td className="text-center">22010000846979</td>
                                           </tr>
                                           <tr>
                                               <td>Ngân hàng TMCP Quân Đội (MB bank)</td>
                                               <td className="text-center">0541102002009</td>
                                           </tr>
                                           </tbody>
                                       </table>

                                   </dl>
                               </>)}

                           </div>
                       }
                   </div>
                </Card.Body>
             
            </Card>
        </>
    )
}

export default Payment;