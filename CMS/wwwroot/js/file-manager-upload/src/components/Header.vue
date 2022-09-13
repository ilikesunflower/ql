<template>
  <div class="muf-header">
    <h1>Featured image</h1>
    <button type="button" class="media-modal-close" @click="closeModel">
      <span class="media-modal-icon"></span>
    </button>
  </div>
</template>

<script>
export default {
  name: "Header",
  props: {
    closeModel:Function
  },
}
</script>
<style type="text/css">div.image {}</style>
<style scoped lang="css">
  button{
    position: absolute;
    top: 0;
    right: 0;
    width: 50px;
    height: 50px;
    margin: 0;
    padding: 0;
    border: 1px solid transparent;
    background: 0 0;
    color: #646970;
    z-index: 1000;
    cursor: pointer;
    outline: 0;
    transition: color .1s ease-in-out,background .1s ease-in-out;
  }
  span.media-modal-icon{
    display: block;
    max-width: 20px;
    max-height: 20px;
    width: 20px;
    background-repeat: no-repeat;
    margin: auto;
    height: 20px;
    background-position: -82px 4px;
    background-size: 115px;
    background-image: url(../assets/uploader-icons.png);
  }
  .muf-header{
    height: 50px;
    padding: 0 16px;
  }
  .muf-header h1{
    font-size: 22px;
    margin: 0px;
    line-height: 2.27272727;
  }
</style>