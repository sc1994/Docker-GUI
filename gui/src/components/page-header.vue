<template>
  <div>
    <el-page-header @back.once="goBack" :content="showName"></el-page-header>
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
      await this.axios.get(`v1/container/cancel/${this.type}`);
      this.$router.push("/");
    }
  }
};
</script>