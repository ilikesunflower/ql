<template>
  <div class="attachments-browser">
    <MediaFilter/>
    <ListAttachments/>
    <MediaSidebar/>
  </div>
</template>

<script>

import MediaFilter from "./MediaFilter.vue";
import ListAttachments from "./ListAttachments.vue";
import MediaSidebar from "./MediaSidebar.vue";

export default {
  name: "AttachmentsBrowser",
  components:{MediaFilter,  MediaSidebar,ListAttachments},
}
</script>

<style scoped lang="css">

</style>