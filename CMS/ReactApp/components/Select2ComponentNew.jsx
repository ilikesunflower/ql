import React, { useState, useEffect, useRef } from 'react';
import PropTypes from 'prop-types';
import Select from 'react-select'

Select2ComponentNew.propTypes = {
    selectKey: PropTypes.string.isRequired,
    selectText: PropTypes.string.isRequired,
    options: PropTypes.array.isRequired
}
function Select2ComponentNew(props) {
    let { options, selectKey, selectText, defaultValue, value, placeholder, onChange, name, isDisabled , className} = props;
    const [optionV, setOption] = useState([]);
    const [defaultV, setDefaultV] = useState({});
    const [nameV, setNameV] = useState(name);
    const [placeholderV, setPlaceholderV] = useState('');
    const formatOption = function (rawOption, key, text, value = 0) {
        let option = [];
        if (Array.isArray(rawOption) && rawOption.length > 0) {
            rawOption.forEach(obj => {
                let opt = { value: obj[key], label: obj[text] };
                option.push(opt);
            })
        }
        return option;
    }

    useEffect(() => {
        let data = formatOption(options, selectKey, selectText, (value ? value : defaultValue));
        let optionSelect = data.find(option =>
            option.value === defaultValue);
        setDefaultV(optionSelect);
        setOption(data)
        setPlaceholderV(placeholder);
        setNameV(name);
    }, [options, defaultValue]);

    let customProps = { ...props };
    delete customProps.options;
    delete customProps.selectKey;
    delete customProps.selectText;
    delete customProps.defaultValue;

    const handleChange = (e) => {
        //   const { props } = this;
        if (!props.onChange) {
            return;
        }
        let data = {
            name: props.name,
            value: e?.value,
            label: e?.label
        }
        props.onChange({
            ...data,
        });
    };
    return (
        <Select placeholder={placeholder} isDisabled={isDisabled} className={className} options={optionV} value={defaultV} onChange={handleChange} />
    )
}
export default Select2ComponentNew;