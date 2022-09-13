import React from 'react';
import NumberFormat from "react-number-format";


function InputNumber(props) {
    let {onValueChange,name} = props
    const onChange = (data) => {
        let value = {
            id:props["data-id"],
            name:name,
            value:data.floatValue
        }
        onValueChange(value);
    }
    return (
        <NumberFormat
            {...props}
            thousandSeparator={','}
            decimalSeparator={'.'}
            autoComplete="off"
            value={!Number.isNaN(Number.parseFloat(props.value)) ? Number.parseFloat(props.value).toLocaleString("en-US", { maximumFractionDigits: 10 }) : 0}
            onValueChange={onChange}
        />
    );
}

export default InputNumber;