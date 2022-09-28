import React , {useEffect, useState, useRef} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import Select from 'react-select'
import "../app.css" 
import MainController from "./MainController";
import SelectNew2 from "../../../components/SelectNew2";
import SelectNew from "../../../components/SelectNew";
import SelectNewMutile2 from "../../../components/SelectNewMutile2";

import {
    InputField,
    TextareaField,
    NumberFormatField,
    NumberFormatFieldAfter,
    FileField,
    CheckBoxField,
    FileFieldP, Textarea, TextareaShort
} from "../../../components/formikField"
import ProductPurposeView from "../../components/ProductPurposeView";

import NumberFormat from "react-number-format";
const optionsSex = [
    {value: 0 , label: "Tất cả"},
    {value: 1 , label: "Nam"},
    {value: 2, label: "Nữ"}
]
const withValueLimit = ({ floatValue }) => floatValue <= 999999999;
function MainApp(props) {
    const {formik, state, method} = MainController();
    let meta = formik.formikProduct.getFieldMeta("checkExitSku") ;
    let meta1 = formik.formikProduct.getFieldMeta("codeStock")
    return (
        <Row>
            <Col md={12}>
                {(state.showPurpose || state.showDeletePurpose )&& 
                    <ProductPurposeView handPurpose={method.handPurpose}
                                        showCreate={state.showPurpose}
                                        showDelete={state.showDeletePurpose}
                                        formik={formik.formikProductPurpose}
                                        handDelete={method.handDeletePurpose}
                                        listProductPurpose={state.listProductPurpose}
                                        clickElement={method.clickElement} 
                                        deletePurpose={method.deletePurpose}
                    />}  
                {state.showCategory && <ModalCategory formik={formik.formikProductCategory} handCategory={method.handCategory} show={state.showCategory}/>}  
                <Card>
                    <Form onSubmit={formik.formikProduct.handleSubmit} >
                        <Card.Header className="row m-0 p-0 border-bottom-0 headerColor">
                            <div className="col-12">
                                <span className="card-title namePageText2 pl-3 pt-3">Thêm mới sản phẩm</span>
                                <div className="float-right pt-3  pr-3">
                                    <Button className="btn-success" type="submit">
                                        Lưu
                                    </Button>
                                </div>
                            </div>
                            <div className="col-12">
                                <ul className="nav nav-tabs" role="tablist">
                                    <li className="nav-item active">
                                        <a className="nav-link active" data-toggle="tab" href="#tab-1" aria-expanded="True">Thông tin cơ bản</a>
                                    </li>
                                    <li className="nav-item">
                                        <a className="nav-link" data-toggle="tab" href="#tab-2" aria-expanded="false"> Mô tả chi tiết</a>
                                    </li>
                                    <li className="nav-item">
                                        <a className="nav-link" data-toggle="tab" href="#tab-3" aria-expanded="false"> Thông số kĩ thuật</a>
                                    </li>
                                </ul>
                            </div>
                        </Card.Header>
                        <Card.Body >
                            <div className="tab-content row">
                                <div id="tab-1" className="tab-pane active col-12">
                                    <div className="row">
                                           <div className="col-xs-12 col-md-8 col-lg-9">
                                               <Card>
                                                   <Card.Body className="row">
                                                       <Form.Group className="col-md-6 pt-3">
                                                           <Form.Label className="form-check-label">Mã hàng  <span className="text-danger">*</span> </Form.Label>
                                                           <InputField className="form-control-xl form-control " placeholder="Nhập mã hàng"  formik={formik.formikProduct} name="sku"/>
                                                       </Form.Group>
                                                       <Form.Group className="col-md-6 pt-3">
                                                           <Form.Label className="form-check-label">Tên mặt hàng  <span className="text-danger">*</span> </Form.Label>
                                                           <InputField className="form-control-xl form-control " placeholder="Nhập tên mặt hàng"  formik={formik.formikProduct} name="name"/>
                                                       </Form.Group>
                                                   
                                                       <Form.Group className="col-md-12 pt-3">
                                                           <Form.Label className="form-check-label ">Mục đích sử dụng  <span className="text-danger">*</span> </Form.Label>
                                                           <SelectNew2  selectKey="value" selectText="label"  options={state.listProductPurpose} defaultValue={formik.formikProduct.values.productPurposeId}  placeholder="Chọn mục đích sử dụng" name="productPurposeId"  formik={formik.formikProduct} handDelete={method.handDeletePurpose}  hand={method.handPurpose} className=" col-md-12" statusDelete={true}/>
                                                       </Form.Group>
                                                       <Form.Group className="col-md-12 pt-3">
                                                           <Form.Label className="form-check-label ">Danh mục sản phẩm  <span className="text-danger">*</span> </Form.Label>
                                                           <SelectNewMutile2 options={state.listProductCategory} formik={formik.formikProduct} name="productCategory" hand={method.handCategory} defaultValue={[]} selectKey="value" selectText="label" placeholder="Chọn danh mục" className="basic-multi-select rounded-0 col-md-11 col-sm-9 pr-0" />
                                                       </Form.Group>
                                                       <div className="d-flex justify-content-center col-12  pt-3">
                                                           <button  type="button" className="btn btn-secondary" onClick={method.onClickImage}  >Upload ảnh phụ</button>
                                                           <input  type="file" onChange={method.handleChangeFile} multiple ref={state.refImage} hidden/>
                                                       </div>
                                                       <div className="col-12  pt-3"> 
                                                           <div className={"row d-flex justify-content-center  " + (state.listFile.length == 0 ? '' : 'borderUploadMany')}>
                                                               {
                                                                   state.listFile.map((x, i) => {
                                                                       return(
                                                                           <div className="col-2 pt-3 pb-3  d-flex justify-content-center " key={i} >
                                                                               <i className="fas fa-minus-circle buttonDelete"  onClick={() =>method.deleteMany(i)}></i>

                                                                               <div className="img">
                                                                                   <img src={x}  className="imgC"/>
                                                                               </div>
                                                                           </div>
                                                                       )
                                                                   })
                                                               }
                                                           </div>
                                                       </div>
                                                       <Form.Group className="col-md-12 pt-3">
                                                           <Card className="">
                                                               <Card.Header style={{fontSize:"18px"}}>
                                                                   Thông tin bán hàng
                                                               </Card.Header>
                                                               <Card.Body>
                                                                   <div className="text-center">
                                                                       {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
                                                                       {meta1.touched && meta1.error ? (<span className="text-danger">{meta1.error}</span>) : null}
                                                                   </div>
                                                                   <div className="row  d-flex justify-content-center">
                                                                        {
                                                                            (state.listProperties.length > 0) 
                                                                            && <Properties deleteT={method.deleteProperties} deleteDetailProperties={method.deleteDetailProperties} addDetailProperties={method.addDetailProperties}  listProperties={state.listProperties} hand11={method.handFormProperties11} hand1={method.handFormProperties1}/>
                                                                        }
                                                                      
                                                                     
                                                                       {(state.listProperties.length < 3) && (
                                                                           <div className="col-12 text-center">
                                                                               <Row className="d-flex justify-content-center">
                                                                                   <div className="col-8">
                                                                                       <Card>
                                                                                               <Button variant="danger" className="btn btnButtonY" onClick={method.addFormProperties} >
                                                                                                   <i className="far fa-plus pr-1"></i>
                                                                                                    Thêm thuộc tính
                                                                                               </Button>  
                                                                                       </Card>
                                                                                   </div>
                                                                               </Row>
                                                                            
                                                                           </div>
                                                                       )}
                                                                       {
                                                                           (state.listProperties.length == 0)
                                                                           && (
                                                                               <div className="col-12 pb-3"  >
                                                                                   <div className="col-12 pb-3" >
                                                                                       <div className="row pb-3"  >
                                                                                           <Form.Label className="col-md-2  ">Tồn kho <span className="text-danger">*</span> </Form.Label>
                                                                                           <NumberFormatField className="form-control-xl form-control col-md-8 "   formik={formik.formikProduct} name="quantityStock" />
                                                                                          
                                                                                       </div>
                                                                                       <div className="row pb-3" >
                                                                                           <Form.Label className="col-md-2  ">Mã kho hàng <span className="text-danger">*</span></Form.Label>
                                                                                           <InputField err={false} className="form-control-xl form-control col-md-8  pl-2 "   formik={formik.formikProduct} name="codeStock" />

                                                                                       </div>
                                                                                       <div className="row">
                                                                                           <Form.Label className=" col-md-2 "> Giá bán</Form.Label>
                                                                                           <NumberFormatFieldAfter className="form-control-xl form-control  col-md-12 "   classnamediv="price_class col-8 pl-0"   placeholder="Nhập giá bán thị trường "  formik={formik.formikProduct} name="price" />
                                                                                       </div>
                                                                                   </div>
                                                                               </div>
                                                                           )
                                                                       }
                                                                       {state.listProperProduct.length > 0 && (
                                                                           <div className="col-12 pb-3"  >
                                                                               <div className="row">
                                                                                   <Form.Label className="col-12  " style={{fontSize:"18px"}}>Danh sách phân loại hàng </Form.Label>

                                                                                   <Col md={12}>
                                                                                       {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
                                                                                       <div className="table-responsive">
                                                                                           <Table className=" table-bordered " >
                                                                                               <thead>
                                                                                               <tr>
                                                                                                   <th >Tên</th>
                                                                                                   <th style={{width: "20%"}}>Mã kho hàng</th>
                                                                                                   <th style={{width: "20%"}}>Giá (đồng)</th>
                                                                                                   <th style={{width: "20%"}}>Tồn kho</th>
                                                                                               </tr>
                                                                                               </thead>
                                                                                               <tbody>
                                                                                               {state.listProperProduct.map((item, i)=> {
                                                                                                   return (
                                                                                                       <tr key={i}>
                                                                                                           <td className="">{item.name}</td>
                                                                                                           <td><input defaultValue={item.skuMh} type='text'  onBlur={(e) => method.handSkuMh(e, i)}  className="form-control-xl form-control " name="skuMh"/></td>
                                                                                                           <td>
                                                                                                               <NumberFormat  className="form-control form-control-xl  "  thousandSeparator={'.'} decimalSeparator={','} name="price" autoComplete="off"
                                                                                                                              value={!Number.isNaN(Number.parseInt(item.price)) ? Number.parseInt(item.price) : 0} onValueChange={(e) => method.handPrice(e,i)}/>
                                                                                                           </td>
                                                                                                           <td>
                                                                                                               <NumberFormat  isAllowed={withValueLimit}  className="form-control form-control-xl  " thousandSeparator={'.'} decimalSeparator={','} name="price" autoComplete="off" 
                                                                                                                              value={!Number.isNaN(Number.parseInt(item.quantity)) ? Number.parseInt(item.quantity) : 0} onValueChange={(e) => method.handQuantitySkuMh(e,i)}/>
                                                                                                           </td>
                                                                                                       </tr>
                                                                                                   )
                                                                                               })}

                                                                                               </tbody>
                                                                                           </Table>
                                                                                       </div>
                                                                                   </Col>
                                                                               </div>
                                                                           </div>
                                                                       )}
                                                                     
                                                                   </div> 
                                                               </Card.Body>
                                                           </Card>
                                                       </Form.Group>
                                                   
                                                   </Card.Body>
                                               </Card>
                                           </div> 
                                            <div className="col-md-4 col-lg-3">
                                                <Card className="file-group">
                                                    <Card.Header>
                                                        <h5 >Ảnh chính <span className="text-danger">*</span> </h5>
                                                    </Card.Header>
                                                    <Card.Body>
                                                        <div className="row">
                                                            <div className="col-lg-12 " onClick={()=> {$(state.refI.current).click()}}>
                                                                <FileFieldP setImageString={method.setImageString} formik={formik.formikProduct} refU={state.refI}  name="image" className="hidden"/>
                                                                <img src={state.imageString} className="imgA"/>
                                                            </div>
                                                         </div>
                                                    </Card.Body>
                                                </Card>
                                                <Card>
                                                    <Card.Body className="row">
                                                        <Form.Group className="col-md-12 pt-3">
                                                            <Form.Label className="form-check-label">Cân nặng</Form.Label>
                                                            <NumberFormatFieldAfter className="form-control-xl form-control " classnamediv="weight_class"   placeholder="Nhập cân nặng"  formik={formik.formikProduct} name="weight" />
                                                        </Form.Group>
                                                        <Form.Group className="col-md-12 pt-3">
                                                            <Form.Label className="form-check-label">Giá gốc</Form.Label>
                                                            <NumberFormatFieldAfter className="form-control-xl form-control "  classnamediv="price_class"    placeholder="Nhập giá bán"  formik={formik.formikProduct} name="priceSale" />
                                                        </Form.Group>
                                                    
                                                        <Form.Group className="col-md-12 pt-3">
                                                            <Form.Label className="form-check-label ">Giới tính   </Form.Label>
                                                            <SelectNew options={optionsSex} defaultValue={formik.formikProduct.values.productSex} formik={formik.formikProduct} name="productSex"   selectKey="value" selectText="label" placeholder="Chọn giới tính"  />
                                                        </Form.Group>
                                                        <Form.Group className="col-md-12 pt-3">
                                                            <Form.Label className="form-check-label">Độ tuổi </Form.Label>
                                                            <NumberFormatField className="form-control-xl form-control " formik={formik.formikProduct}  placeholder="Nhập độ tuổi"   name="productAge" />
                                                        </Form.Group>
                                                        <Form.Group className="col-md-12 pt-3">
                                                            <Form.Label className="form-check-label">Đơn vị  <span className="text-danger">*</span> </Form.Label>
                                                            <InputField className="form-control-xl form-control " formik={formik.formikProduct}  placeholder="Nhập đơn vị"   name="unit"/>
                                                        </Form.Group>
                                                    </Card.Body>
                                                </Card>
                                                
                                                <Card>
                                                    <Card.Body className="row">
                                                     
                                                        <Form.Group className="col-12 pt-3">
                                                            <div className="row">
                                                                <Form.Label className="form-check-label col-9 pr-2 ">Sản phẩm nổi bật </Form.Label>
                                                                <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik.formikProduct} name="isHot"/>
                                                            </div>
                                                        </Form.Group>
                                                        <Form.Group className="col-md-12 pt-3">
                                                            <div className="row">
                                                                <Form.Label className="form-check-label col-9 pr-2 ">Sản phẩm mới </Form.Label>
                                                                <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik.formikProduct} name="isNew"/>
                                                            </div>
                                                        </Form.Group>  
                                                        <Form.Group className="col-md-12 pt-3">
                                                            <div className="row">
                                                                <Form.Label className="form-check-label col-9 pr-2 ">Sản phẩm bán chạy </Form.Label>
                                                                <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik.formikProduct} name="isBestSale"/>
                                                            </div>
                                                        </Form.Group>
                                                        <Form.Group className="col-md-12 pt-3">
                                                            <div className="row">
                                                                <Form.Label className="form-check-label col-9  pr-2 ">Sản phẩm khuyến mãi </Form.Label>
                                                                <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik.formikProduct} name="isPromotion"/>
                                                            </div>
                                                        </Form.Group>
                                                        {/*<Form.Group className="col-md-12 pt-3">*/}
                                                        {/*    <div className="row">*/}
                                                        {/*        <Form.Label className="form-check-label col-9 pr-2  ">Kích hoạt</Form.Label>*/}
                                                        {/*        <CheckBoxField  className=" inputCheckbox pr-1" size="xl" formik={formik.formikProduct} name="isPublic"/>*/}
                                                        {/*    </div>*/}
                                                        {/*</Form.Group>*/}
                                                    </Card.Body>
                                                </Card>
                                           </div> 
                                    </div>
                                </div>
                                <div id="tab-2" className="tab-pane  col-12">
                                    <Card>
                                        <Card.Body>
                                            <Form.Group className="col-md-12 pt-3">
                                                <div className="row">
                                                    <Form.Label className=" col-md-2 pr-2  " style={{fontSize:"18px"}}>Mô tả ngắn</Form.Label>
                                                    <TextareaShort className="col-md-10"  placeholder="Nhập mô tả sản phẩm"   formik={formik.formikProduct} name="lead"/>
                                                </div>
                                            </Form.Group>
                                            <Form.Group className="col-md-12 pt-3">
                                                <div className="row">
                                                    <Form.Label className=" col-md-2 pr-2  " style={{fontSize:"18px"}}>Mô tả</Form.Label>
                                                    <Textarea className="col-md-10" placeholder="Nhập mô tả sản phẩm"   formik={formik.formikProduct} name="description"/>
                                                </div>
                                            </Form.Group>
                                          
                                        </Card.Body>
                                    </Card>
                                </div>
                                <div id="tab-3" className="tab-pane  col-12">
                                    <Card>
                                        <Card.Body>
                                            <Form.Group className="col-md-12 pt-3">
                                                <div className="row">
                                                    <Form.Label className=" col-md-2 pr-2  " style={{fontSize:"18px"}}>Thông số kĩ thuật</Form.Label>
                                                    <Textarea className="col-md-10"  placeholder="Nhập thông số kĩ thuật"   formik={formik.formikProduct} name="specifications"/>
                                                </div>
                                            </Form.Group>
                                        </Card.Body>
                                    </Card>
                                </div>
                            </div>
                        </Card.Body>
                    </Form>
                </Card>
            </Col>
        </Row>
    );
}



const ModalCategory = props => {
    const {handCategory, formik, show} = props;
    return(
        <Modal  show={show}  onHide={handCategory} animation={false}>
            <Form className="form-horizontal" onSubmit={formik.handleSubmit}>
                <Modal.Header >
                    <Modal.Title>Thêm danh mục sản phẩm</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form.Group className="col-md-12">
                        <Form.Label className="form-check-label">Tên  <span className="text-danger">*</span> </Form.Label>
                        <InputField className="form-control-xl form-control " formik={formik} name="name"/>
                    </Form.Group>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handCategory}>
                        Hủy
                    </Button>
                    <Button variant="primary" type="submit">
                        Lưu
                    </Button>
                </Modal.Footer>
            </Form>
        </Modal>
    );
}

const Properties = props => {
    const {hand11, hand1, listProperties, deleteT , addDetailProperties, deleteDetailProperties} = props;
    return(
        <div className="col-12 pb-3"  >
            {listProperties.map((value , index) => {
                return(
                    <div className="col-12 pb-3" key={index} >
                        <div className="row" >
                            <Form.Label className="col-2  ">Nhóm phân loại {index + 1} </Form.Label>
                            <div  className="col-8" >
                                <Card>
                                    <i className="fas fa-minus-circle p-1 buttonDelete" onClick={() => deleteT(index)}></i>
                                    <Card.Body className="row">
                                        <Form.Group className="col-md-12 pt-3">
                                            <div className="row">
                                                <span className="col-md-2  ">Tên nhóm phân loại </span>
                                                <div className="col-md-10">
                                                    <div className="row" >
                                                        <div className="col-11">
                                                            <input    defaultValue={value?.name}  onBlur={(e) => hand1(e, index)}  className="form-control-xl form-control" placeholder="Nhập tên phân loại, Ví dụ size ..." size="xl" name="isHot"/>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </Form.Group>
                                        <div className="col-12">
                                            <div className="row">
                                                <div className="col-md-2 pt-3">
                                                    <span >Phân loại hàng </span>
                                                </div>
                                                <div className="col-md-10 ">
                                                    {(value != null && value?.properties.length > 0 ) && (
                                                        value?.properties.map((x , i)=> {
                                                            return(
                                                                <div className="row" key={i}>
                                                                    <div className="col-11">
                                                                        <input defaultValue={x}  onBlur={(e) => hand11(e, index, i)}  placeholder="Nhập phân loại, Ví dụ S,M" className="form-control-xl form-control mt-3 " size="xl" name="ssd"/>
                                                                    </div>
                                                                    <div className="col-1">
                                                                        {(value?.properties.length != 1) && (<i className="far fa-trash-alt iconTrash pt-4" onClick={() =>deleteDetailProperties(index, i)}></i>)}
                                                                    </div>

                                                                </div>
                                                            )
                                                        })
                                                    )}
                                                </div>

                                                <div className="col-md-12 pt-3  d-flex justify-content-center">
                                                    <Button  variant="danger"  className="btn btnButtonY" onClick={(e) => addDetailProperties(e, index)}  >
                                                        <i className="far fa-plus"></i>
                                                        Thêm phân loại hàng
                                                    </Button>
                                                </div>
                                            </div>
                                        </div>



                                    </Card.Body>
                                </Card>
                            </div>
                        </div>
                    </div>
                )
            })}
        </div>

    );

}
export default MainApp;