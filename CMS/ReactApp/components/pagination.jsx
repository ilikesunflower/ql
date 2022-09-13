import React from "react";
import ReactPaginate from 'react-paginate';

const pagination = (props) => {
    if (props.pageCount > 1) {
        return <nav aria-label="Page navigation example">
                   <ReactPaginate
                       previousLabel="«"
                       nextLabel="»"
                       containerClassName="pagination pagination-sm justify-content-end"
                       breakLinkClassName="page-link"
                       breakClassName="page-item"
                       pageClassName="page-item"
                       previousClassName="page-item"
                       nextClassName="page-item"
                       pageLinkClassName="page-link"
                       previousLinkClassName="page-link"
                       nextLinkClassName="page-link"
                        activeClassName="active"
                        pageCount={props.pageCount}
                        forcePage={props.forcePage}
                       onPageChange={props.handlePageClick} />
               </nav>;
    } else {
        return "";
    }
}

export default pagination;

