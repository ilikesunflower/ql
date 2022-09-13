import React , {useState, useEffect}from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {InputField, TextareaField} from "../../../../components/formikField"
import SelectNew from "../../../../components/SelectNew"
import NumberFormat from "react-number-format";
import {formatNumber } from "../../../../common/app"

function Shipment(props) {
    let {formik, shipmentPartners} = props;
    const shipPartnerProps = formik.getFieldProps("shipPartner");
    const shipPartner = shipPartnerProps.value;

    const shipTypeProps =  formik.getFieldProps("shipType")
    const shipTypeMeta = formik.getFieldMeta("shipType");

    const shipType =  shipTypeProps.value
    return (
        <>
            <Card>
                <Card.Header>
                    <span className="card-title namePageText2 ">Phương thức giao hàng</span>
                </Card.Header>
                <Card.Body >
                    {
                        shipmentPartners.map(shipmentPartner => (
                            <div key={shipmentPartner.type}>
                                <div className="group-radio">
                                    <input id={"radio_ship_"+shipmentPartner.type} type="radio" {...shipPartnerProps} value={shipmentPartner.type} checked={shipmentPartner.type == shipPartner} />
                                    <label className="pl-3" htmlFor={"radio_ship_"+shipmentPartner.type}><i className="radio-icon"></i>{shipmentPartner.name}</label>
                                </div>
                                {shipmentPartner.type == shipPartner && (
                                    <div className="group-select-body">
                                        <div className="card-contain__body">
                                            <ul className="list-group ">
                                                {(shipmentPartner?.shipmentTypes || []).map(shipmentType => {
                                                    return (<li key={shipmentType.type} className="list-group-item borderLi" >
                                                        <input  id={"radio_ship_gh1"+shipmentType.type}  {...shipTypeProps} type="radio" value={shipmentType.type} checked={shipmentType.type == shipType} />
                                                        <label  className="pl-3"  htmlFor={"radio_ship_gh1"+shipmentType.type}>
                                                            <strong >{shipmentType.name}</strong>
                                                            <span > ({formatNumber(shipmentType.cost  || 0)} <u>đ</u>) </span>
                                                            <i className="radio-icon"></i>
                                                        </label>
                                                    </li>)
                                                }  ) }
                                            </ul>
                                            {(shipTypeMeta.touched && shipTypeMeta.error) && (<span className="text-danger">{shipTypeMeta.error}</span>)}
                                        </div>
                                    </div>)}
                            </div>
                        ))
                    }
                </Card.Body>
             
            </Card>
        </>
    )
}

export default Shipment;