import React, {useState, useEffect} from 'react';
import PropTypes from "prop-types";
import Select from 'react-select'

SelectNewMutile2.propTypes = {
    selectKey: PropTypes.string.isRequired,
    selectText: PropTypes.string.isRequired,
    options: PropTypes.array.isRequired,
    defaultValue: PropTypes.array
}
function SelectNewMutile2(props){
    let {options, selectKey, selectText, defaultValue, value, placeholder, className, name,formik, isDisabled, hand} = props;
    const [optionV, setOptionV] = useState([]);
    const [defaultV, setDefaultV] = useState([]);
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
        let data = formatOption(options, selectKey, selectText, (prop.value ? prop.value : defaultValue));
        let data1 = formatOption(defaultValue, selectKey, selectText, (prop.value ? prop.value : defaultValue));
        setOptionV(data);
        setDefaultV(data1);
    }, [options, defaultValue]);
    let customProps = {...props};
    delete customProps.options;
    delete customProps.selectKey;
    delete customProps.selectText;
    delete customProps.defaultValue;
    delete customProps.value;
    const handleSelectChange = function (event) {
        console.log(event)
        formik.setFieldValue(name, event);
    }
    return (
        <>
            <div className="row">
                <div className="col-12">
                    <div className="input-group mb-3 row">
                        <Select isMulti  {...prop} {...customProps}  options={optionV} onChange={handleSelectChange}   />
                        <span className="input-group-append  col-md-1 col-sm-3   pr-0">
                             <button type="button" className="btn btn-default btn-xl buttonFont   col-12 " onClick={hand}>
                                <i className="far fa-plus" ></i>
                            </button>
                        </span>
                    </div>
                    {meta.touched && meta.error ? (<p className="text-danger">{meta.error}</p>) : null}

                </div>
            </div>
        </>
        
    )
}
export default SelectNewMutile2;

