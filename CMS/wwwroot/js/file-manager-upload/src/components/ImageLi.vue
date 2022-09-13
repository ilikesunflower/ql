<template>
  <li tabindex="0" role="checkbox" aria-label="2020-12-27" :checked="file.isActive" data-id="5"
      :class="{attachment:true, 'save-ready':true,selected : file.isActive, details :file.isActive}">
    <div class="attachment-preview js--select-attachment type-image subtype-png landscape">
      <div class="thumbnail">
        <div class="centered">
          <img :src="file.thumbnail" draggable="false" alt="">
        </div>
      </div>
    </div>
    <button type="button" class="check" tabindex="-1">
      <span class="media-modal-icon"></span>
    </button>
  </li>
</template>

<script>
export default {
  name: "ImageLi",
  props:{
    file:Object
  }
}
</script>

<style scoped lang="css">

@media only screen and (max-width: 1300px) {
  .attachments .attachment {
    width: 16.66%;
  }
}
@media only screen and (min-width: 1300px) {
  .attachments .attachment {
    width: 10%;
  }
}
.attachments .attachment {
  position: relative;
  float: left;
  padding: 8px;
  user-select: none;
  display: list-item;
  margin: 0;
  box-sizing: border-box;
  color: #3c434a;
  cursor: pointer;
  list-style: none;
  text-align: center;
}

.attachments .attachment[checked="true"] {
  box-shadow: inset 0 0 0 3px #fff, inset 0 0 0 7px #2271b1;
}
.attachments .attachment .attachment-preview {
  position: relative;
  box-shadow: inset 0 0 15px rgb(0 0 0 / 10%), inset 0 0 0 1px rgb(0 0 0 / 5%);
  background: #f0f0f1;
  box-sizing: content-box;
  cursor: pointer;
}
.attachments .attachment .attachment-preview::before {
  content: "";
  display: block;
  padding-top: 100%;
}
.attachments .attachment .thumbnail {
  overflow: hidden;
  position: absolute;
  margin: 0px;
  top: 0;
  right: 0;
  left: 0;
  bottom: 0;
  box-sizing: content-box;
  opacity: 1;
  transition: opacity .1s;
}
.attachments .attachment .thumbnail::after {
  content: "";
  display: block;
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  box-shadow: inset 0 0 0 1px rgb(0 0 0 / 10%);
  overflow: hidden;
}
.attachments .attachment .thumbnail .centered {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  transform: translate(50%,50%);
}
.attachments .attachment .thumbnail .centered img {
  transform: translate(-50%,-50%);
  top: 0;
  left: 0;
  position: absolute;
  max-height: 100%;
}
.attachments .attachment .check {
  display: none;
  height: 24px;
  width: 24px;
  padding: 0;
  border: 0;
  position: absolute;
  z-index: 10;
  top: 0;
  right: 0;
  outline: 0;
  background: #f0f0f1;
  cursor: pointer;
  box-shadow: 0 0 0 1px #fff, 0 0 0 2px rgb(0 0 0 / 15%);
}
.attachments .attachment[checked="true"] .check{
  background-color: #2271b1;
  box-shadow: 0 0 0 1px #fff, 0 0 0 2px #2271b1;
  display: block;
}
.attachments .attachment.details .check:hover .media-modal-icon {
  background-position: -60px 0;
}
.attachments .attachment .check .media-modal-icon {
  display: block;
  background-position: -21px 0;
  height: 15px;
  width: 15px;
  margin: 5px;
  background-image: url(../assets/uploader-icons.png);
  background-repeat: no-repeat;
}
</style>