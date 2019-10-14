<template>
  <div>
    <el-page-header @back="goBack" content="todo 容器名称"></el-page-header>
  </div>
</template>

<script>
export default {
  props: ["id"],
  data() {
    return {};
  },
  methods: {
    goBack() {}
  },
  created() {
    this.axios.get(`v1/container/add/log/${this.id}`);
  }
};
</script>