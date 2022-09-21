import React from 'react';
import {formatNumber} from '../../../common/app';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";

import MainController from "./MainController";
import NumberFormat from "react-number-format";

function MainView(props) {
    const {method, state} = MainController(props);
    let {handShowDetailProduct,showModelDetailProduct } = props;
    return (
        <Modal show={showModelDetailProduct}  animation={false}  onHide={handShowDetailProduct}>
            <Modal.Body>
                  <span className="titleName"> {state.product?.name}</span> 
                  <hr/>
                <div className="product-form-order pb-3">
                    {state.product?.productProperties?.length > 0 && state.product?.productProperties?.map((item, i) => {
                        let listV = item?.productPropertieValue ?? [];
                        return(
                            <div key={i} className="product-variation pb-3">
                                <label className="product-variation-title">{item.name}</label>
                                <div className="product-items">
                                    {
                                        listV.length > 0 && listV.map((itemV, j) => {
                                            return (
                                            <div key={itemV.nonValue} className="product-item" >
                                                  <input type="radio"   id={"p_"+itemV.id} name={item.id} name_id={item.name} name_value={itemV.value} value={itemV.id} onChange={method.selectProperties} />
                                                 <label  htmlFor={"p_"+itemV.id}>{itemV.value}</label>
                                            </div>
                                            );
                                        })
                                    }
                                </div>
                            </div>
                        );
                    })}
                        <div className="btn-group ">
                            <button type="button" className="btn " onClick={(e) => method.clickQuantityBuy(1)}>
                                <i className="fa-solid fa-minus"></i> 
                            </button>
                            <NumberFormat  className="form-control form-control-xl inputQuantity  " thousandSeparator={'.'} decimalSeparator={','} name="price" autoComplete="off" 
                                 value={!Number.isNaN(Number.parseInt(state.quantityBuy)) ? Number.parseInt(state.quantityBuy) : 0} onValueChange={(e) => method.changeQuantityBuy(e)}/>
                            <button type="button"  className="btn " onClick={(e) => method.clickQuantityBuy(2)}>
                                 <i className="fa-solid fa-plus"></i>
                            </button>
                        </div>
                    <br/>
                        <span className="product-amount-group-quantity-in-stock ">(Còn {formatNumber(state.quantityKW)} sản phẩm)</span>
                    {
                        state.err.length > 0 && (<span className="product-helper-danger">{state.err}</span>)
                    }
             
                </div>
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={handShowDetailProduct} >
                    Hủy
                </Button>
                <Button variant="danger" onClick={(e) => method.saveCart(e)}  >
                    Thêm
                </Button>
            </Modal.Footer>
        </Modal>
    )
}

export default MainView;