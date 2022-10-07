import React, {useEffect, useRef, useMemo, useState} from "react";
import NumberFormat from "react-number-format";
import { Editor } from "@tinymce/tinymce-react";
import   "../../wwwroot/js/file-manager-upload/dist/filemanagerupload"
import CropImageView from "./cropImage/CropImageView"
import {getProductCategory} from "../product/create/service/httpService";
let mediaManager = new MediaManager({
    xsrf: $('input:hidden[name="__RequestVerificationToken"]').val(),
    multiSelect: false,
    type: 1,
    confirmDelete: async (DeleteFile) => {
        let confirm = await Swal.fire({
            title: 'Bạn có chắc chắn xóa dữ liệu này?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            confirmButtonColor: '#ed5565',
            cancelButtonText: 'Thoát'
        });
        if (confirm.value) {
            DeleteFile();
        }
    }
});

let mediaManagerForTiny = new MediaManager({
    xsrf: $('input:hidden[name="__RequestVerificationToken"]').val(),
    multiSelect: false,
    confirmDelete: async (DeleteFile) => {
        let confirm = await Swal.fire({
            title: 'Bạn có chắc chắn xóa dữ liệu này?',
            type: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            confirmButtonColor: '#ed5565',
            cancelButtonText: 'Thoát'
        });
        if (confirm.value) {
            DeleteFile();
        }
    }
});

export const Textarea = function (props) {
    let { formik, name} = props;
    const editorRef = useRef(null);
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    const handChange = (event) => {
        formik.setFieldValue(name, editorRef.current.getContent());
    }
    return (
        <>
           <Editor
               onInit={(evt, editor) => editorRef.current = editor}
               initialValue= {prop.value}
               init={{
                   images_upload_url: 'postAcceptor.php',
                   automatic_uploads: false,
                   plugins: 'link image code table media',
                   menubar: 'table insert',
                   height : "400",
                   width: "100%",
                   content_style: "body { font-family: Open Sans; font-size: 16pt; }",
                   fontsize_formats: '8pt 9pt 10pt 11pt 12pt 14pt 16pt 18pt 20pt 22pt 24pt 26pt 28pt 36pt 48pt 72pt',
                   toolbar: 'fontsizeselect | formatselect | fontselect | undo redo| media | image | bold italic | alignleft aligncenter alignright | code | table tabledelete | tableprops tablerowprops tablecellprops | tableinsertrowbefore tableinsertrowafter tabledeleterow | tableinsertcolbefore tableinsertcolafter tabledeletecol| backcolor forecolor  ',
                   font_formats: "Andale Mono=andale mono,times;" +
                       " Arial=arial,helvetica,sans-serif; " +
                       "Arial Black=arial black,avant garde; " +
                       "Book Antiqua=book antiqua,palatino; " +
                       "Comic Sans MS=comic sans ms,sans-serif;" +
                       " Courier New=courier new,courier; " +
                       "Georgia=georgia,palatino; " +
                       "Helvetica=helvetica;" +
                       " Impact=impact,chicago; Symbol=symbol; " +
                       "Tahoma=tahoma,arial,helvetica,sans-serif; " +
                       "Terminal=terminal,monaco; " +
                       "Times New Roman=times new roman,times; " +
                       "Trebuchet MS=trebuchet ms,geneva; " +
                       "Verdana=verdana,geneva;" +
                       " Webdings=webdings; " +
                       " Open Sans=Open Sans;" +
                       "Wingdings=wingdings,zapf dingbats;" +
                       " Noto Serif=noto serif,Segoe UI=segoe ui",
                   file_picker_callback: function(callback, value, meta) {
                       console.log("upload ảnh hehe")
                       // Provide file and text for the link dialog
                       mediaManagerForTiny.on('select', function (obj) {
                           if (meta.filetype == 'image' && obj.type != 1) {
                               toastr.error("file không được hỗ trợ. file phải là một hình ảnh.", "Thông báo");
                               return;
                           }
                           if (meta.filetype == 'media' && (obj.type != 2 && obj.type != 3)) {
                               toastr.error("file không được hỗ trợ. file phải là một Media.", "Thông báo");
                               return;
                           }
                           callback(obj.url, { text: obj.title });
                       })
                       mediaManagerForTiny.open();
                   }
               }}
                 onBlur={handChange}   />
            {meta.touched && meta.error ? (<p className="text-danger pt-3">{meta.error}</p>) : null}
        </>
    )
}
export const TextareaShort = function (props) {
    let { formik, name} = props;
    const editorRef = useRef(null);
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    const handChange = (event) => {
        formik.setFieldValue(name, editorRef.current.getContent());
    }
    return (
        <>
            <Editor
                onInit={(evt, editor) => editorRef.current = editor}
                initialValue= {prop.value}
                init={{
                    plugins: 'link code ',
                    menubar: 'table insert',
                    contextmenu: 'cut copy paste | template',
                    height : "400",
                    width: "100%",
                    content_style: "body { font-family: Open Sans; font-size: 16pt; }",
                    fontsize_formats: '8pt 9pt 10pt 11pt 12pt 14pt 16pt 18pt 20pt 22pt 24pt 26pt 28pt 36pt 48pt 72pt',
                    toolbar: 'fontsizeselect | formatselect | fontselect | undo redo| | bold italic | alignleft aligncenter alignright | code |backcolor forecolor  ',
                    font_formats: "Andale Mono=andale mono,times;" +
                        " Arial=arial,helvetica,sans-serif; " +
                        "Arial Black=arial black,avant garde; " +
                        "Book Antiqua=book antiqua,palatino; " +
                        "Comic Sans MS=comic sans ms,sans-serif;" +
                        " Courier New=courier new,courier; " +
                        "Georgia=georgia,palatino; " +
                        "Helvetica=helvetica;" +
                        " Impact=impact,chicago; Symbol=symbol; " +
                        "Tahoma=tahoma,arial,helvetica,sans-serif; " +
                        "Terminal=terminal,monaco; " +
                        "Times New Roman=times new roman,times; " +
                        "Trebuchet MS=trebuchet ms,geneva; " +
                        "Verdana=verdana,geneva;" +
                        " Webdings=webdings; " +
                        " Open Sans=Open Sans;" +
                        "Wingdings=wingdings,zapf dingbats;" +
                        " Noto Serif=noto serif,Segoe UI=segoe ui",
             
                }}
                onBlur={handChange}   />
            {meta.touched && meta.error ? (<p className="text-danger pt-3">{meta.error}</p>) : null}
        </>
    )
}
export const TextareaField = function (props) {
    let { formik, name, className, id } = props;
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    const handChange = (event) => {
        console.log(event)
        formik.setFieldValue(name, event.target.value);
    }
    return (
        <>
            <textarea id={id}
                {...prop}
                className={"form-control " + (className || '')}
                      onChange={handChange}
            >
            </textarea>
            {meta.touched && meta.error ? (<p className="text-danger pt-3">{meta.error}</p>) : null}
        </>
    )
}
export const NumberFormatField = function (props) {
    let { formik, name, className } = props;
    let meta = formik.getFieldMeta(name);
    let handleChange = data => {
        const changeEvent = {
            target: {
                name: name,
                value: data.floatValue
            }
        };
        formik.handleChange(changeEvent);
    }
    return (
        <>
                <NumberFormat thousandSeparator={'.'} decimalSeparator={','} className={className} autoComplete="off" {...props}
                              value={!Number.isNaN(Number.parseInt(meta.value)) ? Number.parseInt(meta.value) : 0} onValueChange={handleChange}/>
                {meta.touched && meta.error ? (<span className="text-danger  pl-2">{meta.error}</span>) : null}

            </>
    );
}

export const NumberFormatFieldAfter = function (props) {
    let { formik, name, className, classnamediv } = props;
    let meta = formik.getFieldMeta(name);
    let handleChange = data => {
        const changeEvent = {
            target: {
                name: name,
                value: data.floatValue
            }
        };
        formik.handleChange(changeEvent);
    }
    return (
        <> 
            <div className={classnamediv}>
                <NumberFormat thousandSeparator={'.'} decimalSeparator={','} className={className} autoComplete="off" {...props}
                              value={!Number.isNaN(Number.parseInt(meta.value)) ? Number.parseInt(meta.value) : 0} onValueChange={handleChange}/>
            </div>
            {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
        </>
    );
}
export const InputField = function (props) {
    let { formik, name, type, className , err} = props;
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    let checkErr = err ?? true;
    const handChange = function (e) {
        formik.setFieldValue(name, e.target.value.trimStart());
    }
    return (
        <>
            <input type={type || 'text'} {...prop}  onChange={handChange} className={"form-control" + (className || '')}/>
            {(meta.touched && meta.error  && checkErr) ? (<span className="text-danger" >{meta.error}</span>) : null}
        </>
    );
}
export const InputFileField = function (props) {
    let { formik, name,placeholder,accept, className } = props;
    let meta = formik.getFieldMeta(name);
    const onChange = (event) => {
        let file = event.target.files[0];
        let acceptList = accept.split(",").filter(Boolean)

        if (file && acceptList.some( accept => accept.trim() === file.type ) ){
            formik.setFieldValue(name, file)
        }
    }
    let titleFile = useMemo(function () {

        if (meta.value){
            return meta.value.name
        }else{
            return "Chọn file"
        }
        
    },[placeholder,meta.value])
    return (
        <div  className={ className}>
            <label htmlFor="files" className="pl-1 textFile"> {titleFile || meta.value }</label>
                <input id="files" type="file" style={{visibility:'hidden'}} accept={accept}  onChange={onChange}/> 
            {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
        </div>
    );
}

export const SelectField = function (props) {
    let { formik, name, options, placeholder, className} = props;
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    return (
        <>
            <select {...prop} value={prop.value || ''} className={className}>
                {placeholder && <option value="">{placeholder}</option>}
            {options.map(option => <option key={option.key} value={option.key}>{option.value}</option>)}
            </select>
            {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
        </>
    );
}
export const FileField = function (props) {
    let { formik, name, className } = props;
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    const handleInputChange = function (event) {
        formik.setFieldValue(name, event.target.files[0]);
    }
    return (
        <>
            <input type="file" name={name} onBlur={prop.onBlur} onChange={handleInputChange} className={"form-control input-sm " + (className || '')} />
            {meta.touched && meta.error ? (<p className="text-danger">{meta.error}</p>) : null}
        </>
    )
}
export const FileFieldP = function (props) {
    let { formik, name, className , refU, setImageString} = props;
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);

    const handleInputChange = function (event) {
        let nameFile = event.target.files[0].name;
        let check = "";
        if (nameFile != "") {
            check = nameFile.split('.').pop();
        }
        if (check == "jpg" || check == "jpeg" || check == "gif" || check == "png" ) {
            formik.setFieldValue(name, event.target.files[0]);
            setImageString(URL.createObjectURL(event.target.files[0]))
        }else {
            toastr.error("File không đúng định dạng hình ảnh")
        }
      
    }
    return (
        <>
            <input ref={refU} type="file" name={name} onBlur={prop.onBlur} onChange={handleInputChange} className={"form-control input-sm " + (className || '')} />
            {meta.touched && meta.error ? (<p className="text-danger">{meta.error}</p>) : null}
        </>
    )
}

export const FileFieldCropImage = function (props) {
    let { formik, name, className , setImageString, imageString} = props;
    let refU = useRef(null);
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    let [show, setShow] = useState(false);
    let [src, setSrc] = useState('');
    let [image, setImage] = useState(null);
    useEffect(function () {
        if(!show){
            console.log($(refU))
            console.log($(refU.current)[0].value)
            $(refU.current)[0].value = null;
            // $(refU).target.val(null);
        }
    }, [show]);
    const handleImageChange = function (event) {
        if(!event){
            formik.setFieldValue(name, image);
            setImageString(URL.createObjectURL(image))
        }else{
            formik.setFieldValue(name, event);
            setImageString(URL.createObjectURL(event))
        }
        setShow(false);
    }
    const handleInputChange = function (event) {
        let nameFile = event.target.files[0]?.name;
        let check = "";
        if (nameFile != "") {
            check = nameFile.split('.').pop();
        }
        if (check == "jpg" || check == "jpeg" || check == "gif" || check == "png" ) {
            setImage(event.target.files[0]);
            setSrc(URL.createObjectURL(event.target.files[0]));
            setShow(true);
        }else {
            toastr.error("File không đúng định dạng hình ảnh")
        }

    }

    return (
        <>
            {
                show
                &&
                <CropImageView showCrop={show} setShowCrop={setShow} src={src} handleValue={handleImageChange}/> 
            }
            <div className="col-lg-12 " onClick={()=> {$(refU.current).click()}}>
                <input ref={refU} type="file" name={name} onBlur={prop.onBlur} onChange={handleInputChange}  className={"form-control input-sm " + (className || '')} />
                {meta.touched && meta.error ? (<p className="text-danger">{meta.error}</p>) : null}
                <img src={imageString} className="imgA"/>
            </div>
        </>
    )
}
export const SwitchField = function (props) {
    let { formik, name } = props;
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    return (
        <>
        {options.map(opt => {
            let value = opt.key;
            return (
                <label className="radio-inline" title="" key={opt.key}>
                    <input {...prop} type="radio" value={opt.key} checked={meta.value == value}/>{opt.text}
                </label>
            );
        })}
        {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
        </>
    );
}

export const RadioField = function (props) {
    let { formik, name, options } = props;
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    return (
        <>
            {options.map(opt => {
                let value = opt.key;
                return (
                    <label className="radio-inline" title="" key={opt.key}>
                        <input {...prop} type="radio" value={opt.key} checked={meta.value == value}/>{opt.text}
                    </label>
                );
            })}
            {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
        </>
    );
}

export const CheckBoxField = function (props) {
    let { formik, name,  className, size } = props;
    let meta = formik.getFieldMeta(name);
    let prop = formik.getFieldProps(name);
    return (
        <>
            <input type="checkbox" checked={prop.value} className={className} size={size} onChange={() =>  formik.setFieldValue(name, !prop.value)} />
            {meta.touched && meta.error ? (<span className="text-danger">{meta.error}</span>) : null}
        </>
    );
}