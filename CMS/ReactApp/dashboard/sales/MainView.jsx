import React , {useEffect, useState, useMemo} from 'react';
import {Col, Form, Row,  Card , Table} from "react-bootstrap";
import moment from "moment";
import InputDatePicker from "../../components/inputDatePicker";
import {getChartDataSales} from "./httpService"
import Chart from "./Chart"

const bindFormatMoment = (type) => {
    if (type === "years") {
        return "YYYY"
    }
    if (type === "months") {
        return "MM/YYYY";
    }
    return "DD/MM/YYYY";
}
function MainController(props) {
    let [timeFlow, setTimeFlow] = useState("days");
    let [chartData, setChartData] = useState([]);
    let [fromDate, setFromDate] = useState( moment().subtract(1, 'month').format(bindFormatMoment(timeFlow)));
    let [toDate, setToDate] = useState( moment().format(bindFormatMoment(timeFlow)));
    const convertStringToDate= (time) => {
        let date = moment(time, "DD/MM/YYYY");
        if (!date.isValid()) {
            date = moment(time, "MM/YYYY");
            if (!date.isValid()) {
                date = moment(time, "YYYY");
            }
        }
        return date;
    }
    useEffect(function () {
        let from = moment(fromDate, "DD/MM/YYYY");
        let to = moment(toDate, "DD/MM/YYYY");
        if (!from.isValid() || !to.isValid()) {
            from = moment(fromDate, "MM/YYYY");
            to = moment(toDate, "MM/YYYY");
            if (!from.isValid() || !to.isValid()) {
                from = moment(fromDate, "YYYY");
                to = moment(toDate, "YYYY");
            }
        }
        setFromDate(from.format(bindFormatMoment(timeFlow)));
        setToDate(to.format(bindFormatMoment(timeFlow)));
    }, [timeFlow])
    const bindFormat = (type) => {
        if (type === "years") {
            return "yyyy"
        }
        if (type === "months") {
            return "mm/yyyy";
        }
        return "dd/mm/yyyy";
    }
    const getData = ()=> {
        let obj = {
            TimeFlow : timeFlow,
            DateStart : fromDate,
            DateEnd : toDate
        }
        getChartDataSales(obj, function (rs) {
            setChartData(rs);
        })
    }
    useEffect(() => {
        getData();
    }, [])
    const onSubmitForm = () => {
        getData();
    }
    return {state:{timeFlow, fromDate, toDate, chartData}, method: {setTimeFlow, setFromDate,setToDate, convertStringToDate, bindFormat , onSubmitForm} };
 
}
function MainApp(props) {
   let {state, method} = MainController(props);
    return (
        <Row>
            <Col md={12}>
                <Card>
                        <Card.Header >
                            <h5 className="text-color-default font-weight-bold m-b-0 text-uppercase">BÁO CÁO BÁN HÀNG THEO DOANH SỐ</h5>
                        </Card.Header>
                        <Card.Body>
                            <Form className="row">
                                <Form.Group className="col-md-3">
                                    <div className="btn-group btn-group-toggle w-100" data-toggle="buttons">
                                        <label className={state.timeFlow === "days" ? "btn btn-light active" : " btn btn-light"}>
                                            <input
                                                type="radio"
                                                name="options"
                                                id="option_timeFlow_day"
                                                value="days"
                                                checked={state.timeFlow === "days"}
                                                onChange={e => {
                                                    method.setTimeFlow(e.target.value)
                                                }}
                                                autoComplete="off"/> Ngày
                                        </label>
                                        <label className={state.timeFlow === "months" ? "btn btn-light active" : " btn btn-light"}>
                                            <input
                                                type="radio"
                                                name="options"
                                                id="option_timeFlow_month"
                                                value="months"
                                                checked={state.timeFlow === "months"}
                                                onChange={e => {
                                                    method.setTimeFlow(e.target.value)
                                                }}
                                                autoComplete="off"/> Tháng
                                        </label>
                                    </div>
                                </Form.Group>
                                <Form.Group className="col-md-5">
                                    <div className="input-group">
                                        <InputDatePicker
                                            name="fromDate" value={state.fromDate}
                                            className="form-control"
                                            onChange={e => {
                                                method.setFromDate(e.target.value)
                                                let d = method.convertStringToDate(e.target.value);
                                                let d1 = method.convertStringToDate(state.toDate);
                                                if (d.isValid() && d1.isValid() &&  d.toDate().getTime() > d1.toDate().getTime()){
                                                    method.setToDate(e.target.value);
                                                }
                                            }}
                                            setup={{
                                                orientation: 'bottom',
                                                format: method.bindFormat(state.timeFlow),
                                                forceParse: true,
                                                calendarWeeks: true,
                                                autoclose: true,
                                                language: 'vi',
                                                minViewMode: state.timeFlow
                                            }}
                                        />
                                        <div className="input-group-prepend">
                                            <div className="input-group-text" id="btnGroupAddon">Đến</div>
                                        </div>
                                        <InputDatePicker
                                            name="fromDate"
                                            className="form-control"
                                            value={state.toDate}
                                            onChange={e => {
                                                method.setToDate(e.target.value)
                                            }}
                                            setup={{
                                                orientation: 'bottom',
                                                format: method.bindFormat(state.timeFlow),
                                                forceParse: true,
                                                calendarWeeks: true,
                                                autoclose: true,
                                                language: 'vi',
                                                minViewMode: state.timeFlow,
                                                beforeShowMonth: function (date){
                                                    let formattedDate = moment(date).format('MM/YYYY');
                                                    if (formattedDate.toString() === state.fromDate.toString()){
                                                        return true;
                                                    }else{
                                                        let d = moment(state.fromDate, 'MM/YYYY').toDate();
                                                        let d1 = moment(formattedDate, 'MM/YYYY').toDate();
                                                        if (d1.getTime() >= d.getTime()){
                                                            return true;
                                                        }
                                                    }
                                                    return false;
                                                },
                                                beforeShowDay: function (date) {
                                                    let formattedDate = moment(date).format('DD/MM/YYYY');
                                                    if (formattedDate.toString() === state.fromDate.toString()){
                                                        return true;
                                                    }else{
                                                        let d = moment(state.fromDate, 'DD/MM/YYYY').toDate();
                                                        let d1 = moment(formattedDate, 'DD/MM/YYYY').toDate();
                                                        if (d1.getTime() >= d.getTime()){
                                                            return true;
                                                        }
                                                    }
                                                    return false;
                                                },
                                                beforeShowYear: function (date){
                                                    let formattedDate = moment(date).format('YYYY');
                                                    if (formattedDate.toString() === state.fromDate.toString()){
                                                        return true;
                                                    }else{
                                                        let d = moment(state.fromDate, 'YYYY').toDate();
                                                        let d1 = moment(formattedDate, 'YYYY').toDate();
                                                        if (d1.getTime() >= d.getTime()){
                                                            return true;
                                                        }
                                                    }
                                                    return false;
                                                }
                                                // startDate: method.setToDate(state.fromDate)
                                            }}
                                        />
                                    </div>
                                </Form.Group>
                                <Form.Group className="col-md-4">
                                    <button type="button" className="btn btn-danger btn-sl btn-block"
                                            onClick={method.onSubmitForm}>Truy xuất
                                    </button>
                                </Form.Group>
                            </Form>
                          <div className="row pt-3">
                                  <Chart chartData={state.chartData} />
                          </div>
                        </Card.Body>
                </Card>
          
            </Col>
        </Row>
    );
}
export default MainApp;