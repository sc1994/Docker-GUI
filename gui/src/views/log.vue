<template>
  <div>
    <el-page-header @back="goBack" content="todo 容器名称"></el-page-header>
    <div v-for="log in logList" :key="log">
      {{log}}
    </div>
  </div>
</template>

<script>
import connection from "../plugins/signalR";

export default {
  props: ["id"],
  data() {
    return {
      logList: []
    };
  },
  methods: {
    async goBack() {
      await this.axios.get(`v1/container/cancel/log/${this.id}`);
      this.$router.push("/");
    },
    async getLog() {
      await this.axios.get(`v1/container/add/log/${this.id}`);
    }
  },
  async created() {
    connection.on("monitorLog", log => {
      this.logList.push(log);
    });
    await this.getLog();
  }
};
</script>