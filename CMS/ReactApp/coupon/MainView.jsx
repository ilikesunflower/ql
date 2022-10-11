import React , {useEffect, useState, useMemo} from 'react';
import {Col, Form, Row,  Card , Table} from "react-bootstrap";
import Pagination from "../components/pagination"
import {getCouponCustomer} from "./httpService"
import {formatDateEndCoupon, formatDateTime, formatDateTimeNoHour, formatNumber} from "../common/app"
function MainApp(props) {
    const {id} = props;
    const [pageCount, setPageCount] = useState(0);
    const [pageIndex, setPageIndex] = useState(1);
    const [forcePage, setForcePage] = useState(0);
    const [indexStart, setIndexStart] = useState(1);
    const [listData, setListData] = useState([]);
    const handlePageClick = (data)=> {
        setPageIndex(data.selected + 1)
    }
    useEffect(() => {
        getCouponCustomer({idCustomer: id, page:pageIndex }, function (rs) {
            setListData(rs.content);
            setForcePage(rs.pageIndex - 1);
            setPageIndex(rs.pageIndex);
            setPageCount(rs.pageCount);
            setIndexStart((rs.pageIndex -1) * 20 + 1 );
        })
    }, [pageIndex])
    return (
        <Row>
            <Col md={12}>
                <Card>
                    <Form >
                        <Card.Header >
                            <span className="card-title namePageText ">Danh sách coupon</span>
                        </Card.Header>
                        <Card.Body  className="row">
                            <div className="table-responsive">
                                <Table striped bordered hover size="sm" className=" table-check-all table-bordered-index  table">
                                    <thead className="table-thead">
                                    <tr>
                                        <th className="text-center align-middle"  >STT</th>
                                        <th className="text-center align-middle">Khách hàng</th>
                                        <th className="text-center align-middle" >Mã code</th>
                                        <th className="text-center align-middle">Ngày bắt đầu</th>
                                        <th className="text-center align-middle">Ngày kết thúc</th>
                                        <th className="text-center align-middle">Trạng thái</th>
                                        <th className="text-center align-middle">Giá trị (đồng)</th>
                                    </tr> 
                                    </thead>
                                    <tbody>
                                    {
                                        (listData.length > 0) &&
                                        (listData.map((item, index) => {
                                            return (
                                                <tr key={index}>
                                                    <td className="text-center">{indexStart + index}</td>
                                                    <td className="text-center">{item.customer.userName}</td>
                                                    <td className="text-center">{item.code}</td>
                                                    <td className="text-center">{formatDateTime(item.startTimeUse)}</td>
                                                    <td className="text-center">{formatDateEndCoupon(item.endTimeUse)}</td>
                                                    {
                                                        (item.status == 1)
                                                        && (
                                                        <td className="text-center">
                                                            <span className="status badge bg-info text-dark ">Đã sử dụng</span>
                                                        </td>
                                                        )
                                                    }
                                                    {
                                                        (item.status == 0)
                                                        && (
                                                            <td className="text-center">
                                                                <span className="status badge bg-secondary text-dark">Chưa sử dụng</span>
                                                            </td>
                                                        )
                                                    }
                                                    <td className="text-center">{formatNumber(item.reducedPrice)}</td>
                                                </tr>
                                            )
                                        }))
                                    }
                                    {
                                        (listData.length == 0) &&
                                        <tr>
                                            <td  className="text-center align-middle" colSpan="7">
                                                Không có dữ liệu
                                            </td>
                                        </tr>
                                    }
                                    </tbody>
                                </Table>
                            </div>
                        </Card.Body>
                        {
                            (pageCount > 1 &&
                                <Card.Footer>
                                    <div className="float-right m-t-5">
                                        <Pagination handlePageClick={handlePageClick} pageCount={pageCount} forcePage={forcePage} />
                                    </div>
                                </Card.Footer>  
                             )
                        }
                       
                    </Form>
                </Card>
          
            </Col>
        </Row>
    );
}
export default MainApp;