<template>
  <page-header :name="name" :id="id" type="stats">
    <ve-line :data="chartData"></ve-line>
  </page-header>
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
    return {
      chartData: {
        columns: ["日期", "访问用户", "下单用户", "下单率"], 
        rows: [
          
        ]
      }
    };
  },
  methods: {
    async getStats() {
      await this.axios.get(`v1/container/add/stats/${this.id}`);
    }
  },
  watch: {

  },
  async created() {
    connection.on("monitorStats", log => {
      this.pushLog(log);
    });
  },
  mounted() {}
};
</script>