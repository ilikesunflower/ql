import React, {useState, useEffect} from 'react';
import PropTypes from "prop-types";
import Select from 'react-select'

SelectNew.propTypes = {
    selectKey: PropTypes.string.isRequired,
    selectText: PropTypes.string.isRequired,
    options: PropTypes.array.isRequired
}
function SelectNew(props){
    let {options, selectKey, selectText, defaultValue, placeholder, name,formik,  className, isDisabled} = props;
    const [optionV, setOptionV] = useState([]);
    const [defaultV, setDefaultV] = useState({});
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    const formatOption = function (rawOption, key, text, value= 0){
        let option = [];
        if(Array.isArray(rawOption) && rawOption.length > 0){
            rawOption.forEach(obj => {
                let opt = {value : obj[key], label : obj[text]}
                option.push(opt);
            })
        }
        return option;
    }
    useEffect(() => {
        let data = formatOption(options, selectKey, selectText, ( defaultValue));
        let optionSelect = data.find(option => option.value == defaultValue);
        setDefaultV(optionSelect || null);
        setOptionV(data);
    }, [options, defaultValue]);
    let customProps = {...props};
    delete customProps.options;
    delete customProps.selectKey;
    delete customProps.selectText;
    delete customProps.defaultValue;
    delete customProps.value;
    const handleChange = (e) => {
        formik.setFieldValue(name, e?.value);
    }
    return (
    <>
            <Select  placeholder={placeholder} isDisabled={isDisabled} options={optionV} value={defaultV} onChange={handleChange} />
            {meta.touched && meta.error ? (<p className="text-danger col-12">{meta.error}</p>) : null}
    </>
    )
}
export default SelectNew;

