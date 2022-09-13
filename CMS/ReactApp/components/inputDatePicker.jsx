import React, {useEffect, useLayoutEffect, useRef} from 'react';
import PropTypes from 'prop-types';

const InputDatePicker = props => {
    let {value,onChange,setup,readOnly} = props;
    const inputR = useRef(null);
    useEffect(()=>{
        $(inputR.current).on("show", function (e) {
            var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
            $(".datepicker-dropdown").css("top", top);
        });
    }, [])

    useEffect(()=>{
        $(inputR.current).datepicker('destroy');
        
    },[value] ) 
     
    useEffect(()=>{ 
        if (!readOnly){ 
            $(inputR.current).datepicker('destroy');
            $(inputR.current).datepicker(setup); 
        } 
    },[setup,readOnly] )  
    
    useEffect(() => { 
        $(function () { 
            $(inputR.current)[0].onchange = function (e) {
                onChange(e);
            }
        })
    }, [onChange])
    
    return (
        <div className="input-date-picker">
            <input ref={inputR} autoComplete="off" type="text" {...props} readOnly="readonly" />
            <i className="far fa-calendar-alt icon-date-picker"/>
        </div>
    )
};

InputDatePicker.propTypes = {};

export default InputDatePicker;