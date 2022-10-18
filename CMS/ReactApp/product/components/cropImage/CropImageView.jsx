import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import CropImageController from "./CropImageController"
import CropImage from "../../../components/crop/CropImage";

function CropImageView(props) {
    let {isEdit, listFileOld} = props;
    const {state, method} = CropImageController(props);
    return(
        <>
            {(state.showCropImage) &&(
                state.typeFile == 1 ?
                    <CropImage showCrop={state.showCropImage} nameFile={state.nameI}  typeFile={state.typeImage} setShowCrop={method.setShowCropImage} src={state.imageCrop} handleValue={method.handleCropImage}/>
                    : <CropImage showCrop={state.showCropImage} nameFile={state.nameI}  typeFile={state.typeImage} setShowCrop={method.setShowCropImage} src={state.imageCrop} handleValue={method.handleCropImageNew}/>
            )
            }
            <div className="d-flex justify-content-center col-12  pt-3">
                <button  type="button" className="btn btn-secondary" onClick={method.onClickImage}  >Upload ảnh phụ</button>
                <input  type="file" accept="image/*" onChange={method.handleChangeFile} multiple ref={state.refImage} hidden/>
            </div>
            <div className="col-12  pt-3">
                <div className={"row d-flex justify-content-center  " + ((state.listFile.length > 0 || (isEdit &&
                    listFileOld.length > 0) ) ? 'borderUploadMany' : '')}>
                    {
                        state.listFile.map((x, i) => {
                            return(
                                <div className="col-2 pt-3 pb-3  d-flex justify-content-center " key={i} >
                                    <div className="buttonImage" >
                                        <i className="fas fa-minus-circle buttonDelete"  onClick={() =>method.deleteMany(i)}></i>
                                        <i className="fa-solid fa-scissors buttonCrop" onClick={() => method.cropImageNew(i)}></i>
                                    </div>
                                    <div className="img">
                                        <img src={x}  className="imgC"/>
                                    </div>
                                </div>
                            )
                        })
                    }
                    {isEdit && 
                        listFileOld.map((x, i) => {
                            return(
                                <div className="col-2 pt-3 pb-3  d-flex justify-content-center " key={i} >
                                    <div className="buttonImage">
                                        <i className="fas fa-minus-circle buttonDelete"  onClick={() =>method.deleteManyOld(i)}></i>
                                        <i className="fa-solid fa-scissors buttonCrop" onClick={() => method.cropImage(i)}></i>
                                    </div>
                                    <div className="img">
                                        <img src={x + "?w=100"}  className="imgC"/>
                                    </div>
                                </div>
                            )
                        })
                    }
                </div>
            </div>
          
        </>
    );
}


export default CropImageView;