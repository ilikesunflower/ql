import React , {useEffect, useState, useMemo} from 'react';
import {Col, Form, Row,  Card , Table} from "react-bootstrap";
import moment from "moment";
import InputDatePicker from "../../components/inputDatePicker";
import {getChartDataToProduct} from "./httpService"
import Chart from "./Chart"


function MainController(props) {
    let [chartData, setChartData] = useState([]);
    let [fromDate, setFromDate] = useState( moment().subtract(1, 'month').format( "DD/MM/YYYY"));
    let [toDate, setToDate] = useState( moment().format( "DD/MM/YYYY"));
    let [filterStatus, setFilterStatus] = useState(0);
    let [listStatus, setListStatus] = useState({});
    const convertStringToDate= (time) => {
        return  moment(time, "DD/MM/YYYY");;
    }
    const getData = ()=> {
        let obj = {
            FilterStatus : filterStatus,
            DateStart : fromDate,
            DateEnd : toDate
        }
        getChartDataToProduct(obj, function (rs) {
            setChartData(rs.content);
            setListStatus(rs.filter);
        })
    }
    useEffect(() => {
        getData();
    }, [])
    const onSubmitForm = () => {
        getData();
    }
    return {state:{ fromDate,filterStatus, toDate, chartData, listStatus}, method: {setFromDate,setToDate, onSubmitForm, convertStringToDate, setFilterStatus} };
 
}
function MainApp(props) {
   let {state, method} = MainController(props);
    return (
        <Row>
            <Col md={12}>
                <Card>
                        <Card.Header >
                            <h5 className="text-color-default font-weight-bold m-b-0 text-uppercase">Top 20 mặt hàng có mức độ tiêu thụ tốt</h5>
                        </Card.Header>
                        <Card.Body>
                            <Form className="row">
                                <Form.Group className="col-md-3">
                                    <select className="form-control" value={state.filterStatus} onChange={(e) =>
                                    {method.setFilterStatus(e.target.value)
                                    }
                                    }>
                                        { state.listStatus.length > 0 && 
                                            state.listStatus.map((item, index) => {
                                                return <option key={index} value={index}> {item}</option>
                                            })
                                        }
                                    </select>
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
                                                format:"dd/mm/yyyy",
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
                                                format: "dd/mm/yyyy",
                                                forceParse: true,
                                                calendarWeeks: true,
                                                autoclose: true,
                                                language: 'vi',
                                                minViewMode: state.timeFlow,
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