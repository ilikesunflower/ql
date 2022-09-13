import React , {useEffect, useState, useMemo} from 'react';
import {Col, Form, Row,  Card , Table} from "react-bootstrap";
import moment from "moment";
import InputDatePicker from "../../components/inputDatePicker";
import {getToRating} from "./httpService"
import {formatDateEndCoupon, formatDateTime, formatNumber} from "../../common/app";

function MainController(props) {
    let [data, setData] = useState([]);
    let [fromDate, setFromDate] = useState( moment().subtract(1, 'month').format("DD/MM/YYYY"));
    let [toDate, setToDate] = useState( moment().format("DD/MM/YYYY"));
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

    const getData = ()=> {
        let obj = {
            DateStart : fromDate,
            DateEnd : toDate
        }
        getToRating(obj, function (rs) {
            setData(rs);
        })
    }
    useEffect(() => {
        getData();
    }, [])
   
    return {state:{fromDate, toDate, data}, method: { setFromDate,setToDate, convertStringToDate } };
 
}
function MainApp(props) {
   let {state, method} = MainController(props);
    return (
        <Row>
            <Col md={12}>
                <Card className="">
                        <Card.Header >
                            <h5 className="text-color-default font-weight-bold m-b-0 text-uppercase">Top sản phẩm Rating cao nhiều nhất </h5>
                        </Card.Header>
                        <Card.Body>
                          <div className="row">
                              <div className="table-responsive">
                                  <Table striped bordered hover size="sm" className=" table-check-all table-bordered-index  table">
                                      <thead className="table-thead">
                                      <tr>
                                          <th className="text-center align-middle" style={{width: "89px"}}  >STT</th>
                                          <th className=" align-middle">Mã Sp</th>
                                          <th className=" align-middle" >Tên sản phẩm</th>
                                          <th className=" align-middle text-center" style={{width: "10%"}} >Điểm Rating</th>
                                      </tr>
                                      </thead>
                                      <tbody>
                                      {
                                          (state.data.length > 0) &&
                                          (state.data.map((item, index) => {
                                              return (
                                                  <tr key={index}>
                                                      <td className="text-center">{ 1 + index}</td>
                                                      <td className="">{item.sku}</td>
                                                      <td className="">{item.name}</td>
                                                      <td className="text-right">{Math.round(item.rate / item.rateCount) >= 5 ? 5 : Math.round(item.rate / item.rateCount)}</td>
                                                  </tr>
                                              )
                                          }))
                                      }
                                      {
                                          (state.data.length == 0) &&
                                          <tr>
                                              <td  className="text-center align-middle" colSpan="4">
                                                  Không có dữ liệu
                                              </td>
                                          </tr>
                                      }
                                      </tbody>
                                  </Table>
                              </div>
                          </div>
                        </Card.Body>
                </Card>
          
            </Col>
        </Row>
    );
}
export default MainApp;