import React , {useEffect, useState, PureComponent} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import ReactCrop from 'react-image-crop'
import 'react-image-crop/dist/ReactCrop.css';

class CropImageView extends PureComponent  {
    constructor(props) {
        super(props);
        const {showCrop, setShowCrop,src, handleValue } = props;
        this.state = {
            src: src ?? null,
            crop: {
                unit: '%',
                width: 30,
                aspect: 16 / 9
            },
            showCrop,
            setShowCrop,
            handleValue
        };
    }
    onImageLoaded = (image) => {
        this.imageRef = image;
    };
    onCropComplete = (crop) => {
        this.makeClientCrop(crop);
    };
    async makeClientCrop(crop) {
        if (this.imageRef && crop.width && crop.height) {
            await this.getCroppedImg(
                    this.imageRef,
                    crop,
                    'newFile.jpeg'
                );
        }
    }

    onCropChange = (crop, percentCrop) => {
        // You could also use percentCrop:
        // this.setState({ crop: percentCrop });
        this.setState({ crop });
    };
    getCroppedImg(image, crop, fileName) {
        const canvas = document.createElement('canvas');
        const pixelRatio = window.devicePixelRatio;
        const scaleX = image.target.naturalWidth / image.target.width;
        const scaleY = image.target.naturalHeight / image.target.height;
        const ctx = canvas.getContext('2d');

        canvas.width = crop.width * pixelRatio * scaleX;
        canvas.height = crop.height * pixelRatio * scaleY;

        ctx.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
        ctx.imageSmoothingQuality = 'high';

        ctx.drawImage(
            image.target,
            crop.x * scaleX,
            crop.y * scaleY,
            crop.width * scaleX,
            crop.height * scaleY,
            0,
            0,
            crop.width * scaleX,
            crop.height * scaleY
        );
        

      
            canvas.toBlob(
                (blob) => {
                    if (!blob) {
                        //reject(new Error('Canvas is empty'));
                        console.error('Canvas is empty');
                        return;
                    }
                    blob.name = fileName;
                    let file = new File([blob], fileName,{ lastModified: new Date().getTime(), type: blob.type } ) 
                    this.setState({ file });
                },
                'image/jpeg',
                1
            );
   
    }


    render() {
        const { crop, src, showCrop,setShowCrop ,handleValue, file } = this.state;
        return (
            <>
                <Modal  show={showCrop}  onHide={() => setShowCrop(!showCrop)} animation={false}>
                    <Form >
                        <Modal.Header>
                            <Modal.Title>Cắt ảnh</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            {src && (
                                <ReactCrop
                                    crop={crop}
                                    ruleOfThirds
                                    onComplete={this.onCropComplete}
                                    onChange={this.onCropChange}
                                >
                                    <img src={src} onLoad={this.onImageLoaded} />
                                </ReactCrop>
                            )}

                         
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowCrop(!showCrop)}>
                                Hủy
                            </Button>
                            <Button variant="primary" onClick={() => handleValue(file)} >
                                Lưu
                            </Button>
                        </Modal.Footer>
                    </Form>
                </Modal>

            </>
        );
    }


}


export default CropImageView;