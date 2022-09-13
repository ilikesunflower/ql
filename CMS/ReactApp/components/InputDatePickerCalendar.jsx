import React, {useEffect, useLayoutEffect, useRef, useState} from 'react';
import PropTypes from 'prop-types';

const InputDatePickerCalendar = props => {
    let {value, onChange, setup, readOnly, flow} = props;
    const inputR = useRef(null);
  
    useEffect(()=>{
        $(inputR.current).on("show", function (e) {
            var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
            $(".datepicker-dropdown").css("top", top);
         
        });
    }, [])
    useEffect(()=>{
        $(inputR.current).on("show", function (e) {
            $(".month").each(function (index, element) {
                console.log(flow)
                if (index > 3 && flow ) {
                    let value = $(element).attr("class");
                    let check = value.includes("disabled");
                    if(check) $(element).hide();
                }
            });
        });
    }, [setup])

    useEffect(() => {
        $(inputR.current).datepicker('destroy');
       
    }, [value]) 
  
    useEffect(() => {  
        if (!readOnly) {
            $(inputR.current).datepicker('destroy');
            $(inputR.current).datepicker(setup);
        } 
    }, [setup])

    useEffect(() => {
        $(function () {
            $(inputR.current)[0].onchange = function (e) {
                onChange(e);
            }
        })
    }, [onChange])

    function onClickIcon() { 
        $(inputR.current).focus();
    }
    let customProps = {...props};
    delete customProps.flow;

    return (
        <div className="input-group date">
            <input ref={inputR} readOnly={true} className={"form-control datepicker bg-white"} autoComplete="off" type="text" {...customProps}  />
            <div className="input-group-append" onClick={() => onClickIcon()}>
                <div className="input-group-text"><i className="fa fa-calendar"/></div>
            </div>
        </div>
    )
};

InputDatePickerCalendar.propTypes = {};

export default InputDatePickerCalendar;