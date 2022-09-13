import React, {useEffect, useLayoutEffect, useRef} from 'react';
import PropTypes from 'prop-types';

const InputDateQuarter = props => {
    let {value, onChange, setup, readOnly, name} = props;
    const inputR = useRef(null);

    useEffect(() => {
        $(inputR.current).datepicker('destroy');
    }, [value])

    useEffect(() => {
        if (!readOnly) {
            console.log("setup readonly")
            $(inputR.current).datepicker('destroy');
            $.fn.datepicker.dates['qtrs'] = {
                days: ["Chủ nhật", "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7"],
                daysShort: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
                daysMin: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
                months: ["Quý 1", "Quý 2", "Quý 3", "Quý 4", "", "", "", "", "", "", "", ""],
                monthsShort: ["Quý 1", "Quý 2", "Quý 3", "Quý 4", "", "", "", "", "", "", "", ""],
                today: "Today",
                clear: "Clear",
                format: "mm/dd/yyyy",
                titleFormat: "MM yyyy",
                weekStart: 0
            };
            $(inputR.current).datepicker({ 
                orientation: 'bottom',
                format: "MM yyyy",
                minViewMode: 1,
                autoclose: true, 
                language: "qtrs",
                forceParse: false,
                startDate: new Date(new Date().setFullYear(new Date().getFullYear() - 400)),
                endDate: new Date(new Date().setMonth(new Date().getMonth() - 3))
            }).on("show", function(event) {
                if ($("body").hasClass("layout-navbar-fixed")) {
                    var top = $("#navbar-header").outerHeight(true) + parseInt($(".datepicker-dropdown").css("top"));
                    $(".datepicker-dropdown").css("top", top);
                }
                $(".month").each(function(index, element) {
                    if (index > 3) $(element).hide(); 
                });

            });
        } 
    }, [])

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

    return (
        <div className="input-group date">
            <input  ref={inputR}  type="text" name={name}  defaultValue={value}
                                       className="form-control bg-color-transparent quarter-year-picker-max show" autoComplete="off" placeholder=""
                                       readOnly="" />
            <div className="input-group-append" onClick={() => onClickIcon()}>
                <div className="input-group-text"><i className="fa fa-calendar"/></div>
            </div>
        </div>
    )
};

InputDateQuarter.propTypes = {};

export default InputDateQuarter;