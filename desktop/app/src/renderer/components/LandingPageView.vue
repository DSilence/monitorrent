<template>
  <div>
    <download-page-view></download-page-view>
  </div>
</template>

<script>
  import CurrentPage from './LandingPageView/CurrentPage'
  import Versions from './LandingPageView/Versions'
  import DownloadPageView from './DownloadPage/DownloadPageView'
  export default {
    components: {
      CurrentPage,
      Versions,
      DownloadPageView
    },
    name: 'landing-page'
  }
</script>

<style scoped>
  img {
    margin-top: -25px;
    width: 450px;
  }
</style>
