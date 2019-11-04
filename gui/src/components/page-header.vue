<template>
  <div>
    <el-page-header @back="goBack" :content="showName"></el-page-header>
    <br />
    <slot />
  </div>
</template>

<script>
export default {
  props: ["id", "name", "type"],
  data() {
    return {};
  },
  computed: {
    showName() {
      return this.name.replace(/\+\+/g, "/");
    }
  },
  methods: {
    async goBack() {
      this.$router.go(-1);
    }
  },
  async destroyed() {
    await this.axios.get(`container/cancel/${this.type}`);
  }
};
</script>