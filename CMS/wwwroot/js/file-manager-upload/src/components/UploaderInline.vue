<template>
  <div class="uploader-inline hidden">
    <div class="uploader-inline-content has-upload-message">
      <h2 class="upload-message">No items found.</h2>
      <div class="upload-ui">
        <h2 class="upload-instructions drop-instructions">Drop files to upload</h2>
        <p class="upload-instructions drop-instructions">or</p>
        <button type="button" class="browser button button-hero" >Select Files</button>
      </div>
    </div>
  </div>
</template>

<script>
export default {
name: "UploaderInline"
}
</script>

<style scoped lang="css">
.attachments-browser .uploader-inline {
  margin-bottom: 20px;
  padding: 0;
  position: absolute;
  top: 72px;
  left: 0;
  right: 300px;
  bottom: 0;
  overflow: auto;
  outline: 0;
  text-align: center;
}
.uploader-inline-content {
  position: absolute;
  top: 30%;
  left: 0;
  right: 0;
}
.uploader-inline h2.upload-message {
  font-size: 20px;
  line-height: 1.4;
  font-weight: 400;
  margin: 0;
}
.uploader-inline .has-upload-message .upload-ui {
  margin: 0 0 4em;
}
.uploader-inline .drop-instructions {
  display: block;
  font-size: 14px;
  color: #3c434a;
  font-weight: 400;
}
.uploader-inline button.button.browser.button-hero {
  background: #f6f7f7;
  position: relative;
  z-index: 1;
  color: #0a4b78;
  border-color: #8c8f94;
  box-shadow: none;
  font-size: 14px;
  min-height: 46px;
  line-height: 3.14285714;
  vertical-align: top;
  text-decoration: none;
  cursor: pointer;
  border-width: 1px;
  border-style: solid;
  margin: 0;
  -webkit-appearance: none;
  border-radius: 3px;
  white-space: nowrap;
  box-sizing: border-box;
  padding: 0 36px;
}
.uploader-inline button.button.browser.button-hero:hover {
  background: #f0f0f1;
  border-color: #0a4b78;
  color: #0a4b78;
}

</style>