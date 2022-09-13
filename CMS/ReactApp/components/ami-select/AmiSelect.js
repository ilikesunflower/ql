import React, {useCallback, useEffect, useMemo, useRef, useState} from 'react';
import PropTypes from 'prop-types';
import "./AmiSelect.css"

const AmiSelect = props => {
    let {options, onChange, value,paramValue,paramLabel,paramChildren,paramParent} = props;
    const area = useRef(null);
    const inputSearch = useRef(null);
    const [isShow, setIsShow] = useState(false);
    const [isSelectedAll, setIsSelectedAll] = useState(false);
    const [txtSearch, setTxtSearch] = useState('');

    const handlerOpenOptions = function () {
        inputSearch.current.focus();
        setIsShow(true);
    }

    const handleClickOutside = function (event) {
        if (!area.current.contains(event.target) && isShow === true) {
            event.preventDefault();
            event.stopPropagation();
            inputSearch.current.blur();
            setTxtSearch("");
            setIsShow(false);
        }
    }

    useEffect(() => {
        document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    });
    
    useEffect(() => {
        let values = [];
        for (let option of options) {
            values.push(getValue(option),...getChildValue(getChildren(option)));
        }
        if(value.length === values.length){
            setIsSelectedAll(true)
        }else{
            setIsSelectedAll(false)
        }
    },[value,options]);

    const getValue = useCallback((option) => {
        return option?.[paramValue]
    },[paramValue])
    const getValues = useCallback((_options) => {
        return _options.map( option => getValue(option) )
    },[paramValue,getValue])
    const getLabel = useCallback((option) => {
        return option?.[paramLabel]
    },[paramLabel])
    const getParent = useCallback((option) => {
        if(paramParent == undefined || paramParent == null){
            return null
        }
        return option?.[paramParent]
    },[paramParent])
    const getChildren = useCallback((option) => {
        if(paramChildren == undefined || paramChildren == null){
            return []
        }
        let children = option?.[paramChildren] || [];
        return Array.isArray(children) ? children : []
    },[paramChildren])
    const getChildValue = useCallback((options) => {
        let values = [];
        for (let option of options) {
            values.push(getValue(option),...getChildValue(getChildren(option)));
        }
        return values;
    }, [getChildren, getValue])

    const getAllOptions = useMemo(() => {
        let allOptions = [];
        for (let option of options) {
            allOptions.push(option,...getChildren(option));
        }
        return allOptions
    }, [getChildren,options])
    
    const getAllOptionValues = useMemo(() => {
        let values = [];
        for (let option of options) {
            values.push(getValue(option),...getChildValue(getChildren(option)));
        }
        return values
    }, [getValue,getChildValue,getChildren,options])

    const findByValue = useCallback((value) => {
        return getAllOptions.find(option => {
            return getValue(option) === value;
        });
    },[paramValue,getAllOptions])
    
    const getParentOptions = useCallback((option) => {
        let opts = [];
        let parent = findByValue(getParent(option));
        if(parent != null){
            opts.push(parent,...getParentOptions(parent))
        }
        return opts
    },[paramParent,findByValue])
    
    const removeParent = useCallback((option,_value) => {
        let parent = findByValue(getParent(option));
        if(parent == null){
            return;
        }
        let childValue = getValues(getChildren(parent));
        let isSelectSome = _value.some( val => childValue.some( childVal => childVal === val ) );
        if(isSelectSome){
            return;
        }
        let parentValue = getValue(parent);
        let index = _value.findIndex(item => item === parentValue);
        if (index >= 0) {
            _value.splice(index, 1)
        }
        removeParent(parent,_value)
    },[findByValue,getParent,getValue,getValues])
    
    const addData = useCallback(function (option) {
        let values = [...getValues(getParentOptions(option)),getValue(option),...getChildValue(getChildren(option))];
        onChange([...new Set([...value, ...values])])
    }, [getChildValue, getChildren, getValue,getValues, onChange, value])

    const removeData = useCallback(function (option) {
        let valueClose = [...value];
        let values = [getValue(option),...getChildValue(getChildren(option))];
        for (let v of values) {
            let index = valueClose.findIndex(item => item === v);
            if (index >= 0) {
                valueClose.splice(index, 1)
            }
        }
        removeParent(option,valueClose)
        onChange(valueClose)
    }, [getChildValue, getChildren, getValue, onChange, value,removeParent])
    const isSelected = useCallback(function (option) {
        return value.some(item => item === getValue(option))
    },[getValue, value])
    const handlerChangeData = useCallback(function (option) {
        return function () {
            let optValue = getValue(option)
            if (value.find(item => item === optValue) != null) {
                removeData(option)
                return;
            }
            addData(option)
        }
    }, [addData, getValue, removeData, value])
    
    const handlerClickAll = () => {
        if(isSelectedAll){
            onChange([])
            setIsSelectedAll(false);
        }else{
            onChange(getAllOptionValues)
            setIsSelectedAll(true);
        }
    }

    const optionsSearch = useMemo(() => {
        if (txtSearch == '' || txtSearch == null || txtSearch == undefined) {
            return options;
        }
        return getAllOptions.filter(option => getLabel(option).trim().toUpperCase().includes(txtSearch.trim().toUpperCase()));
    }, [getLabel,getAllOptions, options, txtSearch,getChildren])
    
    return (<div className="ami-select-container" ref={area} onClick={handlerOpenOptions}>
        <div className="ami-select-value">
            <div className="ami-select-selected">
                {value.length > 0 ? <p>Đã chọn {value.length}/{getAllOptionValues.length}</p> : <p>Chưa chọn</p>}
            </div>
            <div className="ami-select-box-search">
                <input ref={inputSearch} className="ami-select-input-search" value={txtSearch} onChange={(e) => {
                    setTxtSearch(e.target.value)
                }}/>
            </div>
        </div>
        {isShow && <ul className="ami-select-options">
            {optionsSearch.length === 0 && <li className="ami-select-item">Không có lựa chọn phù hợp</li>}
            {optionsSearch.length > 0 && 
               <>
                   <li className={'ami-select-item ' + (isSelectedAll ? "selected" : "")} onClick={handlerClickAll}><input readOnly={true} type="checkbox" checked={isSelectedAll} /> Chọn tất cả</li>
                   <BuildOptions options={optionsSearch} getChildren={getChildren} getValue={getValue} getLabel={getLabel} isSelected={isSelected} handlerChangeData={handlerChangeData}/>
               </>
            }
        </ul>}
    </div>);
};

const BuildOptions = props => {
    let {options, isSelected, handlerChangeData,getValue,getLabel,getChildren} = props;
    return options.map(option => (
        <React.Fragment key={getValue(option)}>
            <li className={'ami-select-item ' + (isSelected(option) ? "selected" : "")}
                onClick={handlerChangeData(option)}>
                <input type="checkbox" readOnly={true} checked={isSelected(option)} /> {getLabel(option)}
            </li>
            {getChildren(option).length > 0 &&
                <BuildOptions options={getChildren(option)} getValue={getValue} getChildren={getChildren} getLabel={getLabel} isSelected={isSelected} handlerChangeData={handlerChangeData}/>}
        </React.Fragment>
    ));
}

AmiSelect.propTypes = {
    options: PropTypes.array.isRequired,
    onChange: PropTypes.func.isRequired,
    value: PropTypes.array,
    paramValue: PropTypes.string.isRequired,
    paramLabel: PropTypes.string.isRequired,
    paramChildren: PropTypes.string,
    paramParent: PropTypes.string,
};

export default AmiSelect;
