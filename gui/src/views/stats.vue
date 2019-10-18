<template>
  <page-header :name="name" :id="id" type="stats">
    <div id="c1"></div>
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
      chartData: [
        { genre: "Sports", sold: 275 },
        { genre: "Strategy", sold: 115 },
        { genre: "Action", sold: 120 },
        { genre: "Shooter", sold: 350 },
        { genre: "Other", sold: 150 }
      ]
    };
  },
  methods: {
    async getStats() {
      await this.axios.get(`v1/container/add/stats/${this.id}`);
    }
  },
  watch: {
    "getStats.length"() {
      // Step 2: 载入数据源
      this.chart.source(this.chartData);
      // Step 3：创建图形语法，绘制柱状图，由 genre 和 sold 两个属性决定图形位置，genre 映射至 x 轴，sold 映射至 y 轴
      this.chart
        .interval()
        .position("genre*sold")
        .color("genre");
      this.chart.render();
    }
  },
  async created() {
    connection.on("monitorStats", log => {
      this.pushLog(log);
    });
  },
  mounted() {
    
  }
};
</script>