import React, {useEffect, useState} from 'react';
import {formatNumber} from '../../../common/app';
import {getListProduct} from "./httpService"
function MainController(props) {
    const {setProductSelect, productCartSelect, setProductCartSelect} = props;
    let [listProduct, setListProduct] = useState([]);

    useEffect(() => {
        getListProduct(function (rs) {
            setListProduct(rs);
        })
    }, []);
    const handleChangeSelect = (event) => {
        setProductSelect(event.value)
    };
    return {method:{handleChangeSelect }, state: {listProduct, productCartSelect }};
}

export default MainController;