import React , {useState, useEffect}from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";

import {formatNumber, formatDateEndCoupon} from "../../../../common/app"

function Coupon(props) {
    let {formik, showModalCoupon, handShowModalCoupon, listCoupon} = props
    console.log(listCoupon);
    const couponCode = formik.values.couponCode;
    const applyDiscountCode =  (code) => {
        return function (event) {
            if(event.target.checked){
                formik.setFieldValue("couponDiscount", code?.reducedPrice)
                formik.setFieldValue("couponCode",  code?.code)
            }
        }
    }
    const isDate = (dateTime) => {
        const today = (new Date().getTime()) + (86400000 * 7)
        const preSevenDay = new Date(today);
        const date = new Date(dateTime);
        return  preSevenDay > date ;
    }
    const removeVoucher =  () => {
        formik.setFieldValue("couponDiscount", 0)
        formik.setFieldValue("couponCode",  '')
    }
    return (
        <Modal show={showModalCoupon}  animation={false}  onHide={handShowModalCoupon}>
            <Modal.Header>
                <span className="titleName">Mã voucher </span>
                <a className="modal-coupon__close"  onClick={handShowModalCoupon}><i className="fa-solid fa-xmark"></i></a>

            </Modal.Header>
            <Modal.Body>
                <div className="overflow-auto modalComment  ">
                    <ul className="modal-coupon__list">
                        { listCoupon?.map(discountCode => {
                            console.log("dfkjsdkf", couponCode === discountCode.code)
                            return (<li key={discountCode.id} className="modal-coupon__item">
                                <input id={"radio_sort"+discountCode.id} checked={couponCode === discountCode.code} onChange={applyDiscountCode(discountCode)} name="radio_sort" type="radio"/>
                                <label htmlFor={"radio_sort"+discountCode.id} className="modal-coupon__item-content">
                                    <i className="radio-icon"></i>
                                    <div className="modal-coupon__item-logo"></div>
                                    <div className="modal-coupon__item-info">
                                        <p className="modal-coupon__item-title"><strong>{discountCode?.code}</strong>{isDate(discountCode?.endTimeUse) && <span>(Sắp hết hạn)</span>}</p>
                                        <p className="modal-coupon__item-discount">Giảm {formatNumber(discountCode?.reducedPrice)} <u>đ</u></p>
                                        <span className="modal-coupon__item-date">Hạn dùng: {formatDateEndCoupon(discountCode?.endTimeUse)}</span>
                                    </div>
                                </label>
                            </li>)
                        }
                           ) }
                        {listCoupon.length === 0 && <li className="modal-coupon__item">
                            <p className="empty-text text-center">
                                Quý khách chưa có mã giảm giá nào
                            </p>
                        </li>}
                </ul>
                </div>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="danger" onClick={removeVoucher}  >
                    Hủy bỏ voucher
                </Button>
            </Modal.Footer>
        </Modal>
    )
}

export default Coupon;