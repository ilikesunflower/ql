<template>
  <div class="media-uploader-status" >
    <div class="upload-details" :class="{ success : file.status === 1 , failure : file.status === 2 , uploading: file.status === 0}">
      <span class="upload-preview">
        <img :src="file.urlPreview">
      </span>
      <div class="upload-info">
        <span class="upload-filename">{{file.name}}</span>
        <div class="media-progress-bar" :style="{display: file.status === 0? '' :'none', }">
          <div :style="{width:file.process+'%'}"></div>
        </div>
        <span class="upload-message" :style="{display: file.status !== 0? '' :'none', }">
          {{file.message}}
        </span>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: "FileBeingUploaded",
  props:{
    file:Object
  }
}
</script>

<style scoped lang="css">
  .upload-info{
    text-align: left;
    flex: 1;
    padding-right: 10px;
  }
  .upload-info .upload-filename{
    font-weight: 600;
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    width: 300px;
    display: block;
  }  
  .upload-info .upload-message{
    display: block;
  }
 
  .media-uploader-status{
    padding: 5px 10px;
    position: relative;
    margin: 0 auto;
    max-width: 400px;
  }
  .media-progress-bar{
    width: 100%;
    position: relative;
    height: 10px;
    margin: 10px auto;
    border-radius: 10px;
    background: #dcdcde;
    background: rgba(0,0,0,.1);
  }
  .media-progress-bar div{
    height: 10px;
    min-width: 20px;
    width: 0;
    background: #2271b1;
    border-radius: 10px;
    transition: width .3s;
  }
  .upload-details{
    display: flex;
  }
  .upload-details.success{
    border-left: 3px solid #1ab34d;
    background-color: #f3fff5;
  }
  .upload-details.failure{
    border-left: 3px solid  #ED5666;
    background-color: #FCF0F1;
  }
  .upload-details.uploading{
    border-left: 3px solid  #1D84C6;
    background-color: #edf7ff;
  }
  .upload-details .upload-preview{
    margin: 0 10px 0 0;
    width: 55px;
    height: 50px;
    background-color: #d0d0d0;
    overflow: hidden;
  }
  .upload-details .upload-preview img{
    height: 50px;
  }
</style>