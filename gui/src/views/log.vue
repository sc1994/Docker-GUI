<template>
  <el-page-header @back="goBack" content="详情页面">{{id}}</el-page-header>
</template>

<script>
export default {
  props: ["id"],
  data() {
    return {};
  },
  methods: {
    goBack() {}
  }
};
</script>