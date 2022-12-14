import React , {useState, useEffect}from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import {formatNumber } from "../../../common/app"
import {checkShipmentCost} from "./httpService";

function Shipment(props) {
    let {formik,productCartSelect} = props;
    const [shipmentPartners, setShipmentPartners] = useState([{
        name: "Nhận hàng tại kho",
        shipmentTypes: [],
        type: 0
    }, {
        name : "Đối tác khác",
        shipmentTypes : [],
        type : 3
    }]);
    const shipPartnerProps = formik.getFieldProps("shipPartner");
    const shipPartner = shipPartnerProps.value;

    const shipTypeProps =  formik.getFieldProps("shipType")
    const shipTypeMeta = formik.getFieldMeta("shipType");

    const shipType =  shipTypeProps.value

    const getShipCost = async function () {
        let { provinceCode, districtCode, communeCode} = formik.values;

        if (productCartSelect.length === 0) {
            return;
        }
        let shipmentCost = [
            {
                name: "Nhận hàng tại kho",
                shipmentTypes: [],
                type: 0
            },
            {
                name : "Đối tác khác",
                shipmentTypes : [],
                type : 3
            }];
        let weight = productCartSelect.reduce((previousValue, currentValue) => previousValue + (currentValue.weight * currentValue.quantityBy|| 0), 0);
        formik.setFieldValue("totalWeight", weight)
        if ( provinceCode && districtCode && communeCode) {
            let param = {provinceCode, districtCode, communeCode, weight};
            checkShipmentCost(param , function (rs) {
                shipmentCost = rs?.shipmentPartners || [];
                setShipmentPartners(shipmentCost);
            })
        }
        setShipmentPartners(shipmentCost);
    }

    useEffect(function () {
        getShipCost()
    }, [
        productCartSelect,
        formik.values.provinceCode,
        formik.values.districtCode,
        formik.values.communeCode,
    ])
    useEffect(function () {
        let shipPartner = shipmentPartners.find(partner => partner.type == formik.values.shipPartner);
        if (shipPartner) {
            let shipmentType = shipPartner?.shipmentTypes.find(shipmentType => shipmentType.type == formik.values.shipType);
            formik.setFieldValue("priceNoSale", shipmentType?.cost || 0);
        }else{
            formik.setFieldValue("priceNoSale",0);
        }
    }, [
        shipmentPartners,
        formik.values.shipPartner,
        formik.values.shipType
    ])
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
                                                    let disable = !shipmentType.cost;
                                                    return (<li key={shipmentType.type} className="list-group-item borderLi" >
                                                        <input disable={disable ? 1 : 0} id={"radio_ship_gh1"+shipmentType.type}  {...shipTypeProps} type="radio" value={shipmentType.type} checked={shipmentType.type == shipType} />
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