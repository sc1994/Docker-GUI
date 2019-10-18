<template>
  <page-header :name="name" :id="id" type="stats">123123</page-header>
</template>

<script>
import pageHeader from "../components/page-header";
import connection from "../plugins/signalR";

export default {
  components: {
    pageHeader
  },
  props: ["id", "name"],
  data() {
    return {};
  },
  methods: {},
  async created() {
    connection.on("monitorLog", log => {
      this.pushLog(log);
    });

    await this.axios.get(`v1/container/add/stats/${this.id}`);
  }
};
</script>