<script setup>
  import NavBar from './components/NavBar.vue'
  import DatePicker from 'primevue/datepicker'
</script>


<template>
  <div class="container-fluid">
    <div class="row">
      <NavBar></NavBar>
    </div>
    <router-view></router-view>
  </div>
  <DatePicker id="false"></DatePicker>
</template>


<style scoped>
.app-container {
  background-color: #422439;
  background-size: cover;
  background-position: center;
  min-height: 100vh;
  margin: 0;
  padding: 0%;
  display: flex;
  flex-direction: column;
}
#false{
  display:none;
}
</style>

<style>
.p-datepicker-panel {
  z-index: 2000 !important;
}
</style>