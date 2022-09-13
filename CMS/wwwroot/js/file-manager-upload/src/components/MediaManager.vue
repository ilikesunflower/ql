<template>
  <div class="muf-root">
    <div class="muf-modal">
      <Header :closeModel="closeModel"/>
      <Body :emitEvent="emitEvent" :closeModel="closeModel"/>
      <Footer/>
    </div>
    <div class="muf-modal-backdrop" @click="closeModel"></div>
  </div>
</template>

<script>
import Header from "./Header.vue";
import Body from "./Body.vue";
import Footer from "./Footer.vue";
export default {
  name: "MediaManager",
  setup(props,context) {
    const rootModel = document.getElementById(context.attrs.rootId);
    const closeModel = ()=>{
      rootModel.style.display = 'none';
    }
    return {
      closeModel
    }
  },
  props: {
    emitEvent:Function
  },
  components:{
    Header,
    Body,
    Footer
  }

}
</script>
<style scoped lang="css">
  .muf-modal{
    background:#ffffff;
    position: fixed;
    top: 30px;
    left: 30px;
    right: 30px;
    bottom: 30px;
    z-index: 1500;
  }
  .muf-modal-backdrop{
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    min-height: 360px;
    background: #000;
    opacity: .7;
    z-index: 1499;
  }
</style>