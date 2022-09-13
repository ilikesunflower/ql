<template>
  <ul tabindex="-1" class="attachments ui-sortable ui-sortable-disabled" id="attachments-view">
    <ImageLi v-for="(file, index ) in StateListFiles" :key="index" :file="file" @click="activeFiles(file)" />
  </ul>
</template>

<script>
import {mapActions, mapGetters} from 'vuex'
import ImageLi from "./ImageLi.vue";
export default {
  name: "ListAttachments",
  components:{ImageLi},
  methods:{
    ...mapActions(['activeFiles','loadMoreFiles'])
  },
  mounted () {
    const listFiles = document.querySelector('#attachments-view');
    listFiles.addEventListener('scroll', e => {
      if(listFiles.scrollTop + listFiles.clientHeight >= listFiles.scrollHeight - 1) {
        this.loadMoreFiles();
      }
    });
  },
  computed: {
    ...mapGetters(['StateListFiles'])
  }
}
</script>

<style scoped lang="css">
.attachments-browser .attachments, .attachments-browser .uploader-inline {
  position: absolute;
  top: 72px;
  left: 0;
  right: 300px;
  bottom: 0;
  overflow: auto;
  outline: 0;
  margin: 0;
  padding: 2px 8px 8px;
}
</style>