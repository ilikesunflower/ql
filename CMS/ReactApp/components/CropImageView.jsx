import React , {useEffect, useState, PureComponent} from 'react';
import {Col, Form, Row,  Card, Table, Modal , Button } from "react-bootstrap";
import ReactCrop from 'react-image-crop'
class CropImageView extends PureComponent  {
    constructor(props) {
        const {showCrop, setShowCrop,src } = props;
        super(props);
        this.state = {
            src: src ?? null,
            crop: {
                unit: '%',
                width: 30,
                aspect: 16 / 9
            },
            showCrop: showCrop,
            setShowCrop: setShowCrop
        };
    }
 
    onImageLoaded = (image) => {
        console.log("onImageLoaded", image)
        this.imageRef = image;
    };
    onCropComplete = (crop) => {
        this.makeClientCrop(crop);
    };
    async makeClientCrop(crop) {
        if (this.imageRef && crop.width && crop.height) {
            const croppedImageUrl = await this.getCroppedImg(
                this.imageRef,
                crop,
                'newFile.jpeg'
            );
            this.setState({ croppedImageUrl });
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
        const scaleX = image.naturalWidth / image.width;
        const scaleY = image.naturalHeight / image.height;
        const ctx = canvas.getContext('2d');

        canvas.width = crop.width * pixelRatio * scaleX;
        canvas.height = crop.height * pixelRatio * scaleY;

        ctx.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
        ctx.imageSmoothingQuality = 'high';

        ctx.drawImage(
            image,
            crop.x * scaleX,
            crop.y * scaleY,
            crop.width * scaleX,
            crop.height * scaleY,
            0,
            0,
            crop.width * scaleX,
            crop.height * scaleY
        );

        return new Promise((resolve, reject) => {
            canvas.toBlob(
                (blob) => {
                    if (!blob) {
                        //reject(new Error('Canvas is empty'));
                        console.error('Canvas is empty');
                        return;
                    }
                    blob.name = fileName;
                    window.URL.revokeObjectURL(this.fileUrl);
                    this.fileUrl = window.URL.createObjectURL(blob);
                    resolve(this.fileUrl);
                },
                'image/jpeg',
                1
            );
        });
    }


    render() {
        console.log(this.state);
        const { crop, croppedImageUrl, src , showCrop, setShowCrop} = this.state;
        return (
            <>
                <Modal  show={showCrop}  onHide={() => setShowCrop(!showCrop)} animation={false}>
                    <Form >)
                        <Modal.Header>
                            <Modal.Title>Cắt ảnh</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            {src && (
                                <div>
                                    <p>djd</p>
                                    <ReactCrop
                                        src={src}
                                        crop={crop}
                                        ruleOfThirds
                                        onImageLoaded={this.onImageLoaded}
                                        onComplete={this.onCropComplete}
                                        onChange={this.onCropChange}
                                    >  
                                       
                                </ReactCrop>
                            </div>
                               
                            )}
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowCrop(!showCrop)}>
                                Hủy
                            </Button>
                            <Button variant="primary" >
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