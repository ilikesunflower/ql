import React, { useState } from "react";
import Cropper from "react-cropper";
import "cropperjs/dist/cropper.css";
import "./CropImage.css";
import {Button, Form, Modal} from "react-bootstrap";

 function CropImage (props) {
     const {showCrop, setShowCrop,src, handleValue,typeFile, nameFile } = props;
    const [image, setImage] = useState(src);
    const [cropper, setCropper] = useState({});
    const getCropData = () => {
        if (typeof cropper !== "undefined") {

            cropper.getCroppedCanvas().toBlob(
                    (blob) => {
                        if (!blob) {
                            //reject(new Error('Canvas is empty'));
                            console.error('Canvas is empty');
                            return;
                        }
                        blob.name = nameFile;
                        let file = new File([blob], nameFile,{ lastModified: new Date().getTime(), type:"image/"+ typeFile} )
                        handleValue(file)
                    },
                    'image/' + typeFile,
                    1
                );
        }
    };
    return (
     <>
         <Modal  show={showCrop}  animation={false}>
             <Form >
                 <Modal.Header>
                     <Modal.Title>Cắt ảnh</Modal.Title>
                 </Modal.Header>
                 <Modal.Body className="modal-body body-crop-image">
                     {src ? (
                         <Cropper
                             style={{ height: 400, width: "100%" }}
                             zoomTo={0}
                             preview=".img-preview"
                             src={image}
                             viewMode={1}
                             background={false}
                             responsive={true}
                             autoCropArea={1}
                             aspectRatio={16 / 16}
                             checkOrientation={false} // https://github.com/fengyuanchen/cropperjs/issues/671
                             onInitialized={(instance) => {
                                 setCropper(instance);
                             }}
                             guides={true}
                         />
                     ) : <p>Không có ảnh</p>}


                 </Modal.Body>
                 <Modal.Footer>
                     <Button variant="secondary" onClick={() => setShowCrop(!showCrop)}>
                         Hủy
                     </Button>
                     <Button variant="primary" onClick={() => getCropData()} >
                         Lưu
                     </Button>
                 </Modal.Footer>
             </Form>
         </Modal>

     </>
    );
};

export default CropImage;
