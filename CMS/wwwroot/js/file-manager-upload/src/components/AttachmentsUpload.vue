<template>
  <div class="uploader-inline">
    <div class="uploader-inline-content no-upload-message">
      <div class="upload-ui">
        <h2 class="upload-instructions drop-instructions">Drop files to upload</h2>
        <p class="upload-instructions drop-instructions">or</p>
        <button type="button" class="browser button button-hero" @click="triggerClickUploadFile">
          Select Files
        </button>
      </div>
      <div class="upload-inline-status">
        <FileBeingUploaded v-for="index in ListFileBeingUploaded" :file="StateFileBeingUploaded[index]" :key="index"/>
      </div>
    </div>
    <input id="file-upload" type="file" ref="file" @change="inputFileOnChange" multiple="multiple"  accept="image/jpeg,.jpg,.jpeg,.jpe,image/gif,.gif,image/png,.png,image/bmp,.bmp,image/tiff,.tiff,.tif,.ico,.heic,.asf,.asx,video/x-ms-wmv,.wmv,.wmx,.wm,video/avi,.avi,.divx,video/x-flv,.flv,video/quicktime,.mov,.qt,video/mpeg,.mpeg,.mpg,.mpe,video/mp4,.mp4,video/x-m4v,.m4v,video/ogg,.ogv,video/webm,.webm,video/x-matroska,.mkv,video/3gpp,.3gp,.3gpp,video/3gpp2,.3g2,.3gp2,text/plain,.txt,.asc,.c,.cc,.h,.srt,text/csv,.csv,.tsv,.ics,.rtx,text/css,.css,text/html,.htm,.html,.vtt,.dfxp,audio/mpeg,.mp3,audio/x-m4a,.m4a,.m4b,audio/aac,.aac,.ra,.ram,audio/x-wav,.wav,audio/ogg,.ogg,.oga,audio/flac,.flac,.mid,.midi,audio/x-ms-wma,.wma,.wax,.mka,text/rtf,.rtf,application/x-javascript,.js,application/pdf,.pdf,.class,.tar,application/zip,.zip,.gz,.gzip,.rar,.7z,image/photoshop,.psd,.xcf,application/msword,.doc,application/vnd.ms-powerpoint,.pot,.pps,.ppt,.wri,.xla,application/vnd.ms-excel,.xls,.xlt,.xlw,.mdb,.mpp,application/vnd.openxmlformats-officedocument.wordprocessingml.document,.docx,.docm,application/vnd.openxmlformats-officedocument.wordprocessingml.template,.dotx,.dotm,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,.xlsx,.xlsm,.xlsb,.xltx,.xltm,.xlam,application/vnd.openxmlformats-officedocument.presentationml.presentation,.pptx,.pptm,application/vnd.openxmlformats-officedocument.presentationml.slideshow,.ppsx,.ppsm,application/vnd.openxmlformats-officedocument.presentationml.template,.potx,.potm,.ppam,.sldx,.sldm,.onetoc,.onetoc2,.onetmp,.onepkg,.oxps,.xps,.odt,.odp,.ods,.odg,.odc,.odb,.odf,.wp,.wpd,.key,.numbers,.pages">
  </div>
</template>

<script>
import {mapActions ,mapGetters} from 'vuex';
import FileBeingUploaded from './FileBeingUploaded.vue'
export default {
  name: "AttachmentsUpload",
  components: {FileBeingUploaded},
  data() {
    return {
      files: {}
    }
  },
  computed:{
    ...mapGetters(['StateFileBeingUploaded']),
    ListFileBeingUploaded(){
      let listFile = Object.keys(this.StateFileBeingUploaded).reverse();
      return listFile;
    }
  },
  mounted() {
    this.setDefaultsFileBeingUploaded();
  },
  methods: {
    ...mapActions(['uploadFile','setDefaultsFileBeingUploaded']),
    triggerClickUploadFile() {
      document.getElementById('file-upload').click()
    },
    inputFileOnChange() {
      this.files = this.$refs.file.files;
      for(let file of this.files){
        let url = URL.createObjectURL(file);
        this.uploadFile({
          file : file,
          url: url
        });
      }
    }
  }
}
</script>

<style scoped lang="css">

#file-upload {
  display: none;
}

.media-frame-content .uploader-inline {
  margin-bottom: 20px;
  padding: 0;
  text-align: center;
}

.uploader-inline .uploader-inline-content {
  position: absolute;
  text-align: center;
  top: 30%;
  left: 0;
  right: 0;
}
.upload-inline-status{
  max-height: 120px;
  overflow: auto;
}
.uploader-inline-content .upload-ui {
  margin: 2em 0;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Oxygen-Sans, Ubuntu, Cantarell, "Helvetica Neue", sans-serif !important;
  color: #1d2327 !important;
  line-height: 1.4;
  font-weight: 400 !important;
}

.muf-media-frame-content .uploader-inline .drop-instructions {
  display: block;
  font-size: 20px;
  line-height: 1.4;
  font-weight: 400;
  margin: 0;
}

.muf-media-frame-content .uploader-inline .button {
  background: #f6f7f7;
  position: relative;
  font-size: 14px;
  display: inline-block !important;
  min-height: 46px;
  line-height: 3.14285714;
  padding: 0 36px;
  color: #2271b1;
  border-color: #2271b1;
  vertical-align: top;
  cursor: pointer;
  text-decoration: none;
  border-width: 1px;
  border-style: solid;
  -webkit-appearance: none;
  border-radius: 3px;
  white-space: nowrap;
  margin: 0;
  box-sizing: border-box;
}

</style>