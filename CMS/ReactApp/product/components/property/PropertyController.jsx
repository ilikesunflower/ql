import React , {useEffect,  useState, useRef} from 'react';



function PropertyController(props) {
    //PropertyController
    let {listProperties, setListProperties, listProperProduct, setListProperProduct, isEdit, checkEditPro, setCheckEditPro} = props;
    useEffect(function () {
        console.log(listProperties);
        
        let data = [];
        let product = {...listProperProduct};
        let dataC = listProperties.filter(x => x.name != '' && fitterTrimArrayString(x.properties).length != 0) || [];
        if(checkEditPro && isEdit){
            if(Array.isArray(dataC) && dataC.length > 0){
                let count = dataC.length;
                if(count == 1){
                    let  valueP = fitterTrimArrayString(dataC[0].properties);
                    valueP.forEach((obj, index) => {
                        let dataP = {
                            name:  obj?.value,
                            skuMh: product[index]?.skuMh || '',
                            price: 0,
                            quantity: 0
                        }
                        data.push(dataP)
                    })
                }else if(count > 1){

                    let valueP = createListProductProperties(dataC,fitterTrimArrayString(dataC[0].properties),1, count)
                    valueP.forEach((obj, index) => {
                        let dataP = {
                            name:  obj,
                            skuMh:  product[index]?.skuMh || '',
                            price: 0,
                            quantity: 0
                        }
                        data.push(dataP)
                    })
                }
            }
            setListProperProduct(data);
            
        }
        if(!isEdit){
            if(Array.isArray(dataC) && dataC.length > 0  ){
                let count = dataC.length;
                if(count == 1){
                    let  valueP = fitterTrimArrayString(dataC[0].properties);
                    valueP.forEach(obj => {
                        let dataP = {
                            name:  obj?.value || '',
                            skuMh: '',
                            price: 0,
                            quantity: 0
                        }
                        data.push(dataP)
                    })
                }
                else if(count > 1){
                    let valueP = createListProductProperties(dataC,fitterTrimArrayString(dataC[0].properties),1, count)
                    valueP.forEach(obj => {
                        let dataP = {
                            name:  obj,
                            skuMh: '',
                            price: 0,
                            quantity: 0
                        }
                        data.push(dataP)
                    })
                }
            }
            setListProperProduct(data);
        }
    

    }, [listProperties]);
    const fitterTrimArrayString = function (lisObj) {
        let value = lisObj.filter(x => x?.value != '');
        return value;
    }
    const createListProductProperties = (listPs, listPro, index2, count) => {
        let pro2 = fitterTrimArrayString(listPs[index2].properties);
        let properties = [];
        listPro.forEach(obj => {
            pro2.forEach(obj2 => {
                properties.push( (index2== 1? obj?.value : obj) + ' _ ' + obj2?.value );
            })
        })
        let indexNext = index2 + 1;

        if(indexNext == count){
            return properties;
        }else{
            return createListProductProperties(listPs,properties, indexNext , count);
        }
    }

    const maxIndexProperties = function (listProperties){
        let indexMax = 0;
        if(Array.isArray(listProperties) && listProperties.length > 0 ){
            let properties = listProperties.reduce(function (previous, current) {
                return (previous.ord > current.ord ? previous : current)
            })
            indexMax = properties.ord + 1;
        }
        return indexMax;
    }
    const deleteProperties = function (property){
        if(isEdit) setCheckEditPro(true);
        let data =[...listProperties] ;
        let index = data.findIndex(x => x.ord == property.ord);
        data.splice(index, 1);
        setListProperties(data);
    }
    const deleteDetailProperties = function ( property1, property2) {
        if(isEdit) setCheckEditPro(true);

        let data = [...listProperties];
        let index = data.findIndex(x => x?.ord == property1?.ord);
        let index1 = property1.properties.findIndex(x => x.ord == property2.ord);
        data[index].properties.splice(index1, 1);
        setListProperties(data);
    }
    const addDetailProperties = function (e, property){
        let data =[...listProperties] ;
        if(isEdit) setCheckEditPro(true);

        let ordNew = maxIndexProperties(property.properties)
        let valueNew = {ord: (ordNew || 1), value: ''}
        let index = data.findIndex(x => x.ord == property.ord);
        data[index].properties.push(valueNew);
        setListProperties(data);
    }
    const handFormProperties1 = function (e, property){
        if(isEdit) setCheckEditPro(true);

        let value =  e.target.value.trim();
        let data =[...listProperties] ;
        let check = data.findIndex(x => x.name == value && x.name != '');
        let index = data.findIndex(x => x.ord == property?.ord);
        if(check >= 0 && check != index){
            data.splice(index, 1);
            setListProperties(data);
            toastr.error("Thuộc tính này đã tồn tại")
        }else{
            data[index].name = value;
            setListProperties(data);
        }
    }
    const handFormProperties11 = function (e, property, property1){
        if(isEdit) setCheckEditPro(true);

        let value = e.target.value.trim();
        if(value != ''){

            let data = [...listProperties];
            let index = data.findIndex(x => x?.ord == property?.ord);
            let check = data[index]?.properties.findIndex(x => x?.value == value);
            let index1 = property.properties.findIndex(x => x?.value == property1?.value);
            if(check > -1 &&  check != index1 ){
                e.target.value = '';
                toastr.error("Thuộc tính này đã tồn tại")
            }else{
                data[index].properties[index1].value = value;
                setListProperties(data);
            }
        }
    }
    const addFormProperties = function (){
        let ord = maxIndexProperties(listProperties);
        if(isEdit) setCheckEditPro(true);

        let val = {
            ord : ord,
            name: '',
            properties: [{ord: 0, value: ''}]
        }
        setListProperties([...listProperties, val]);
    }
    const handSkuMh = function (e, index) {
        let data = [...listProperProduct];
        let value = e.target.value.trim();
        let check = data.findIndex(x => x.skuMh != '' && x.skuMh == value );
        if(check > -1 && check != index){
            e.target.value= '';
            toastr.error("Mã hàng này đã tồn tại")
        }else{
            data[index].skuMh = value;
            setListProperProduct(data);
        }
    }
    const handPrice = function (e, index) {
        let data = [...listProperProduct];
        if(e.floatValue >= 0){
            data[index].price = e.floatValue;
            setListProperProduct(data);
        }
    }
    const handQuantitySkuMh = function (e, index) {
        let data = [...listProperProduct];
        if(e.floatValue <= 999999999 && e.floatValue >= 0){
            data[index].quantity = e.floatValue;
            setListProperProduct(data);
        }
    }
    return {
        state:{
     
        },
        method:{
            deleteProperties,
            deleteDetailProperties,
            addDetailProperties,
            handFormProperties1,
            handFormProperties11,
            addFormProperties,
            handSkuMh,
            handPrice,
            handQuantitySkuMh
    } };
}
export default PropertyController;