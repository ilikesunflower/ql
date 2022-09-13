<template>
  <div class="muf-body">
    <div class="muf-media-frame-router">
      <div role="tablist" aria-orientation="horizontal" class="muf-media-router">
        <button type="button" role="tab" class="muf-media-menu-item" :data-active="tabActive===0 ? 'true': 'false'" id="menu-item-upload" @click="tabActive=0">Upload files</button>
        <button type="button" role="tab" class="muf-media-menu-item" :data-active="tabActive===1 ? 'true': 'false'" id="menu-item-browse" @click="tabActive=1">Media Library</button>
      </div>
    </div>
    <div class="muf-media-frame-content">
      <AttachmentsUpload v-if="tabActive===0" />
      <AttachmentsBrowser v-else/>
    </div>
    <MediaFrameToolbar :emitEvent="emitEvent" :closeModel="closeModel"/>
  </div>
</template>

<script>
import AttachmentsBrowser from "./AttachmentsBrowser.vue";
import AttachmentsUpload from "./AttachmentsUpload.vue";
import MediaFrameToolbar from "./MediaFrameToolbar.vue";
import {mapActions} from "vuex";
export default {
  name: "Body",
  data() {
    return {
      tabActive: 1
    }
  },
  mounted() {
    this.fetchFiles();
  },
  methods:{
    ...mapActions(['fetchFiles'])
  },
  props: {
    emitEvent:Function,
    closeModel:Function
  },
  components:{
    AttachmentsBrowser,
    AttachmentsUpload,
    MediaFrameToolbar
  }
}
</script>

<style scoped lang="css">
  .muf-media-frame-router{
    position: absolute;
    top: 50px;
    left: 0;
    right: 0;
    height: 36px;
    z-index: 200;
  }
  .muf-media-router {
    position: relative;
    padding: 0 6px;
    margin: 0;
    clear: both;
  }
  .muf-media-router .muf-media-menu-item {
    position: relative;
    float: left;
    border: 0;
    margin: 0;
    padding: 8px 10px 9px;
    height: 18px;
    line-height: 1.28571428;
    font-size: 14px;
    text-decoration: none;
    background: 0 0;
    cursor: pointer;
    box-sizing: content-box;
    transition: none;
  }
  .muf-media-router [data-active="true"], .media-router .muf-media-menu-item[data-active="true"]:last-child {
    margin: -1px -1px 0;
    background: #fff;
    border: 1px solid #dcdcde;
    border-bottom: none;
  }
  .muf-media-frame-content {
    position: absolute;
    top: 84px;
    left: 0px;
    right: 0;
    bottom: 61px;
    height: auto;
    width: auto;
    margin: 0;
    overflow: auto;
    background: #fff;
    border-top: 1px solid #dcdcde;
  }
</style>