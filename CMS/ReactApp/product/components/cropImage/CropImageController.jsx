import React , { useState, useRef} from 'react';



function CropImageController(props) {
    //CropImageController
    let {listFileSave, setListFileSave, listFileOld, setListFileOld} = props;
    let [listFile, setListFile] = useState([]);

    let refImage = useRef(null);

    let [showCropImage, setShowCropImage] = useState(false);
    let [imageCrop, setImageCrop] = useState('');
    let [indexImage, setIndexImage] = useState(null);
    let [typeImage, setTypeImage] = useState('');
    let [nameI, setNameI] = useState('');
    let [typeFile, setTypeFile] = useState(0);

    const cropImageNew = function (i) {
        setTypeFile(2);
        let data = [...listFile];
        let data1 = [...listFileSave];
        let img = data[i];
        let img1 = data1[i].name;
        setImageCrop(img);
        setIndexImage(i);
        setShowCropImage(true);
        let typeImg = img1.split('.').pop();
        setNameI(img1)
        setTypeImage(typeImg);

    }
    const onClickImage = function () {
        $(refImage.current).click();
    }
    const handleCropImage = function (event){
        if(!event) return;
        let data = [...listFileOld];
        data.splice(indexImage, 1);
        setListFileOld(data);
        let data1 = [...listFile];
        let data2 = [...listFileSave];
        data2.push(event);
        data1.push(URL.createObjectURL(event));
        setListFileSave(data2);
        setListFile(data1);
        setShowCropImage(false)
    }
    const handleCropImageNew = function (event){
        if(!event) return;
        let data = [...listFile];
        let data1 = [...listFileSave];
        data[indexImage] = URL.createObjectURL(event);
        data1[indexImage] = event;
        setListFileSave(data1);
        setListFile(data);
        setShowCropImage(false)
    }
    const handleChangeFile = function (e) {
        let value = [];
        let value1 = [];
        if( e.target.files.length > 0){
            for (let i = 0; i < e.target.files.length; i++) {
                let nameFile = e.target.files[i].name;
                let check = "";
                if (nameFile != "") {
                    check = nameFile.split('.').pop();
                }
                if (check == "jpg" || check == "jpeg" || check == "gif" || check == "png" ) {
                    value.push(e.target.files[i]);
                    value1.push(URL.createObjectURL( e.target.files[i]));
                }else {
                    toastr.error("File không đúng định dạng hình ảnh")
                }
            }
            setListFileSave([...listFileSave,  ...value]);
            setListFile([...listFile,...value1]);
        }
    }
    const deleteMany = function (i) {
        let data = [...listFile];
        let data1 = [...listFileSave];
        data.splice(i, 1);
        data1.splice(i, 1);
        setListFile(data);
        setListFileSave(data1);
    }
    const deleteManyOld = function (i) {
        let data = [...listFileOld];
        data.splice(i, 1);
        setListFileOld(data);
    }
    const cropImage = function (i) {
        setTypeFile(1);
        let data = [...listFileOld];
        let img = data[i];
        setImageCrop(img);
        let listName = img.split('/').pop();
        let check = listName.split('.').pop();
        setNameI(listName)
        setTypeImage(check);
        setIndexImage(i);
        setShowCropImage(true);
    }
    return {
        state:{
            listFile,
            showCropImage,
            imageCrop,
            indexImage,
            typeImage,
            nameI,
            refImage,
            typeFile
        },
        method:{
            setShowCropImage,
            cropImageNew,
            handleCropImageNew,
            handleCropImage,
            handleChangeFile,
            onClickImage,
            deleteMany,
            deleteManyOld,
            cropImage
    } };
}
export default CropImageController;