import React, {useState, useEffect} from 'react';
import PropTypes from "prop-types";
import Select from 'react-select'
import {CL} from "../../wwwroot/theme/admin-lte/plugins/pdfmake/pdfmake";

SelectNew2.propTypes = {
    selectKey: PropTypes.string.isRequired,
    selectText: PropTypes.string.isRequired,
    options: PropTypes.array.isRequired
}
function SelectNew2(props){
    let {options, selectKey, selectText, defaultValue, value, placeholder, name,formik,  className, isDisabled, hand, statusDelete, handDelete} = props;
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
        let data = formatOption(options, selectKey, selectText, (defaultValue));
        let optionSelect = data.find(option => option.value == defaultValue);
        setDefaultV(optionSelect || null);
        setOptionV(data);
    }, [options, defaultValue]);
    let customProps = {...props};
    delete customProps.options;
    delete customProps.selectKey;
    delete customProps.selectText;
    delete customProps.defaultValue;
    const handleChange = (e) => {
        formik.setFieldValue(name, e?.value);
    }
   
    return (
    <div className="row">
        <div className="col-12">
                <div className="input-group mb-3 row">
                    <Select placeholder={placeholder} isDisabled={isDisabled} className={" rounded-0 col-md-10 col-sm-8 pr-0" } options={optionV} value={defaultV} onChange={handleChange} />
                    <span className="input-group-append  col-md-2 col-sm-4   pr-0">
                               {
                                   (statusDelete) && (
                                       <button type="button" className="btn btn-default btn-xl buttonFont  col-6" onClick={handDelete}>
                                           <i className="fal fa-solid fa-trash"></i>
                                       </button>
                                   )
                               }
                        <button type="button" className="btn btn-default btn-xl   col-6  " onClick={hand}>
                            <i className="far fa-plus"></i>
                        </button>
                    </span>
                </div>

                {meta.touched && meta.error ? (<p className="text-danger col-12">{meta.error}</p>) : null}
        </div>
    </div>
    )
}
export default SelectNew2;

