import React, {useEffect, useState} from 'react';
import {formatNumber} from '../../../common/app';
import {getDetailProduct} from "./httpService"

function MainController(props) {
    const {id, productCartSelect, setProductCartSelect, handShowDetailProduct, formik} = props;
    const [product, setProduct] = useState({});
    const [quantityKW, setQuantityKW] = useState(0);
    const [price, setPrice] = useState(0);
    const [listProductPropertiesSelect, setListProductPropertiesSelect] = useState([]);
    const [quantityBuy, setQuantityBuy] = useState(1);
    const [productSimilarSelect, setProductSimilarSelect] = useState(0);
    const [err, setErr] = useState("");
    
    useEffect(() => {
        getDetailProduct({id: id}, (rs) => {
            setProduct(rs);
            setQuantityKW(rs.quantityKW);
            setPrice(0);
            if ( rs.productProperties.length == 0 && rs.productSimilar.length == 1 ){
                setProductSimilarSelect(rs.productSimilar[0].id);
                setPrice(rs.productSimilar[0].price);
            }
        });
    }, []);
    const method = {
        isValid : (checkQuantity = true) => {
            let isCheck = true;
            if ( productSimilarSelect > 0 ){
                isCheck = isCheck && true;
                setErr("");
            }else{
                isCheck = isCheck && false;
                setErr("Vui lòng chọn phân loại hàng");
            }
            if ( checkQuantity && isCheck ){
                if ( quantityBuy <=  quantityKW && quantityKW != 0){
                    isCheck = isCheck && true;
                    setErr("");
                }else{
                    isCheck = isCheck && false;
                    setErr("Số lượng mua vượt số lượng hàng có sẵn, vui lòng chọn lại");
                }   
            }
            
            return isCheck;
        },
        saveCart: async (e) => {
            if ( method.isValid() ){
                let check = productCartSelect.findIndex(x => x.productId == id && x.productSimilarId == productSimilarSelect);
                if (check > -1 ){
                    setErr("Sản phẩm và loại sản phẩm này đã tồn tại");

                }else{
                    let  listProperties = [];
                    listProductPropertiesSelect.forEach(x => {
                        listProperties.push({
                            propertiesName: x.nameId,
                            propertiesValueName : x.nameValue
                        })
                    })
                    console.log(product)
                    let param = {
                        productId :  id,
                        productSimilarId :  productSimilarSelect,
                        nameProduct: product.name,
                        image: product.image,
                        listProperties: listProperties,
                        quantityWH :  quantityKW,
                        quantityBy :  quantityBuy,
                        price: price,
                        weight: product.weight || 0
                        
                    };
                    let data = [...productCartSelect, param];
                    setProductCartSelect(data);
                    handShowDetailProduct();
                    formik.setFieldValue("checkChangeP", true)
                }
            }
            },
        selectProperties: async (e) => {
            let listProductSimilar = product?.productSimilar ?? [];
            let listProductProperties= product?.productProperties ?? [];
            let obj = {
                id: e.currentTarget.name,
                value: e.currentTarget.value,
                nameId: e.target.attributes.getNamedItem('name_id').value,
                nameValue:e.target.attributes.getNamedItem('name_value').value
            }
            let checkIndex = listProductPropertiesSelect.findIndex(x => x.id == obj.id);
            if ( checkIndex > -1 ) {
                listProductPropertiesSelect.splice(checkIndex, 1);
            }
            listProductPropertiesSelect.push(obj);
            if (listProductPropertiesSelect.length ==  listProductProperties.length){
                let index = listProductSimilar.findIndex(x => {
                    if ( x.productPropertiesValue != null && x.productPropertiesValue.length > 0 ){
                        let a = x.productPropertiesValue.split(",");
                        let isCheck = true;
                        listProductPropertiesSelect.forEach(s => {
                            let c = a.findIndex(x => x == s.value);
                            isCheck = isCheck && (c > -1);
                        })
                        return isCheck;
                    }
                })
                if ( index > -1 ){
                    let productSimilar = listProductSimilar[index];
                    setPrice(productSimilar?.price);
                    setQuantityKW(productSimilar?.quantityWh);
                    setProductSimilarSelect(productSimilar?.id);
                }else{
                    setProductSimilarSelect(0);
                }
            }
        },
        changeQuantityBuy: (e) => {
            if ( method.isValid(true) ){
                let quantity = Math.floor(e.floatValue);
                if ( quantity > quantityKW ){
                    setQuantityBuy(quantityKW);
                }else if ( quantity >= 1 && quantity <= quantityKW){
                    setQuantityBuy(quantity);
                }else{
                    setQuantityBuy(1);
                }
            }
        },
        clickQuantityBuy: (type) => {
            if ( method.isValid(true) ){
                if ( type == 1 ){
                    let q = quantityBuy - 1;
                    if ( q < 1 ){
                        setQuantityBuy(1);
                    }else{
                        setQuantityBuy(q);
                    }
                }else if(type == 2){
                    let q = quantityBuy + 1;
                    if ( q > quantityKW ){
                        setQuantityBuy(quantityKW);
                    }else{
                        setQuantityBuy(q);
                    }
                }   
            }
        }
    };
    return {method, state: {product, quantityBuy, quantityKW, productSimilarSelect ,price, err}}
}

export default MainController;