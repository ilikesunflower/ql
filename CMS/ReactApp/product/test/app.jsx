import React, { PureComponent }  from 'react';
import { createRoot } from 'react-dom/client';
import ReactCrop from 'react-image-crop';
import 'react-image-crop/dist/ReactCrop.css';
const container = document.getElementById('root');
const root = createRoot(container);
class App extends PureComponent {
    state = {
        src: "https://upload.wikimedia.org/wikipedia/commons/thumb/5/57/Galunggung.jpg/450px-Galunggung.jpg",
        crop: {
            unit: "%",
            width: 30,
            aspect: 16 / 9
        }
    };
    // If you setState the crop in here you should return false.
    onImageLoaded = (image) => {
        this.imageRef = image;
    };

    onCropComplete = (crop) => {
        this.makeClientCrop(crop);
    };

    onCropChange = (crop, percentCrop) => {
        // You could also use percentCrop:
        // this.setState({ crop: percentCrop });
        this.setState({ crop });
    };

    async makeClientCrop(crop) {
        console.log("on crop complete", crop, this.imageRef)

        if (this.imageRef && crop.width && crop.height) {
            const croppedImageUrl = await this.getCroppedImg(
                this.imageRef,
                crop,
                "newFile.jpeg"
            );
            console.log("croppedImageUrl",croppedImageUrl)
            this.setState({ croppedImageUrl });
        }
    }

    getCroppedImg(image, crop, fileName) {
        console.log("get cropped img", image, crop, fileName)
        console.log("naturalWidth",image.target)
        const canvas = document.createElement("canvas");
        const pixelRatio = window.devicePixelRatio;
        const scaleX = image.target.naturalWidth / image.target.width;
        const scaleY = image.target.naturalHeight / image.target.height;
        const ctx = canvas.getContext("2d");

        canvas.width = crop.width * pixelRatio * scaleX;
        canvas.height = crop.height * pixelRatio * scaleY;

        ctx.setTransform(pixelRatio, 0, 0, pixelRatio, 0, 0);
        ctx.imageSmoothingQuality = "high";

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

        return new Promise((resolve, reject) => {
            canvas.toBlob(
                (blob) => {
                    if (!blob) {
                        //reject(new Error('Canvas is empty'));
                        console.error("Canvas is empty");
                        return;
                    }
                    blob.name = fileName;
                    window.URL.revokeObjectURL(this.fileUrl);
                    this.fileUrl = window.URL.createObjectURL(blob);
                    resolve(this.fileUrl);
                },
                "image/jpeg",
                1
            );
        });
    }

    render() {
        const { crop, croppedImageUrl, src } = this.state;
        return (
            <div className="App">
              
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
                {croppedImageUrl && (
                    <img alt="Crop" style={{ maxWidth: "100%" }} src={croppedImageUrl} />
                )}
            </div>
        );
    }
}
root.render(
    <App />
);





