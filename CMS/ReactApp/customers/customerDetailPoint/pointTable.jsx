import React, {useEffect, useState} from 'react';
import {FetchPoint} from "./service/PointService";
import {dataFormat} from "../../helpers/dataTimeHelper";
import Pagination from "../../components/pagination";

function PointStatus({endTime}) {
    const date = new Date(endTime);
    const now = new Date();
    
    if (date && date.getTime() < now.getTime()){
        return <span className="status badge bg-danger text-dark">Hết hạn</span>
    }
    return <span className="status badge bg-success text-dark">Khả dụng</span> 
}

function PointTable() {
    const [points, setPoints] = useState([]);
    const [page, setPage] = useState(1);
    const [perPage, setPerPage] = useState(20);
    const [pageCount, setPageCount] = useState(1);

    const fetchPoint = async () => {
        try {
            let response = await FetchPoint(customerId,page);
            if (response.msg === "successful") {
                let {content, pageCount, perPage} = response.data
                if (content && Array.isArray(content)) {
                    setPoints(content);
                }
                setPageCount(pageCount)
                setPerPage(perPage)
            }
        } catch (e) {

        }
        return "Fetch point"
    }

    const handlePageClick = ({selected}) => {
        setPage(selected+1)
    }
    
    useEffect(function () {
        fetchPoint().then(console.log)
    }, [page])

    return (
        <div className="card">
            <div className="card-header">
                <p className="card-title namePageText">Danh sách điểm</p>
            </div>
            <div className="card-body">
                <div className="table-responsive">
                    <table
                        className="table tab-content table-bordered table-check-all table-hover  table-bordered-index  ">
                        <thead className="table-thead">
                        <tr>
                            <th className="text-center align-middle" style={{width: "45px"}}>STT</th>
                            <th className="text-center align-middle">Điểm</th>
                            <th className="text-center align-middle">Số điểm nạp</th>
                            <th className="text-center align-middle">Số điểm trừ</th>
                            <th className="text-center align-middle">Ngày bắt đầu</th>
                            <th className="text-center align-middle">Ngày kết thúc</th>
                            <th className="text-center align-middle">Trạng thái</th>
                        </tr>
                        </thead>
                        <tbody>
                        {points.length > 0 && points.map((point, index) => <tr key={point.id}>
                            <td className="text-center">{(page - 1) * perPage + (index + 1)}</td>
                            <td className="text-right">{point.point}</td>
                            <td className="text-right">{point.addPoint}</td>
                            <td className="text-right">{point.minusPoint}</td>
                            <td className="text-center">{dataFormat(point.startTime)}</td>
                            <td className="text-center">{dataFormat(point.endTime)}</td>
                            <td className="text-center"><PointStatus endTime={point.endTime} /></td>
                        </tr>)}
                        {points.length === 0 && <tr>
                            <td className="text-center" colSpan={7}>Không có dữ liệu</td>
                        </tr>}
                        </tbody>
                    </table>
                </div>
                <Pagination pageCount={pageCount} forcePage={page - 1} handlePageClick={handlePageClick} />
            </div>
        </div>
    );
}

export default PointTable;