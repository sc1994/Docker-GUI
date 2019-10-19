<template>
  <page-header :name="name" :id="id" type="stats">
    <el-row :gutter="20">
      <el-col :span="12">
        <ve-line :data="chartCpu" :extend="extend"></ve-line>
      </el-col>
      <!-- <el-col :span="12">
        <ve-line :data="chartCpu" :extend="extend"></ve-line>
      </el-col>-->
    </el-row>
    <!-- <el-row :gutter="20">
      <el-col :span="12">
        <ve-line :data="chartCpu" :extend="extend"></ve-line>
      </el-col>
      <el-col :span="12">
        <ve-line :data="chartCpu" :extend="extend"></ve-line>
      </el-col>
    </el-row>-->
  </page-header>
</template>

<script>
import connection from "../plugins/signalR";
import pageHeader from "../components/page-header";
var dateFormat = require("dateformat");

export default {
  components: {
    pageHeader
  },
  props: ["id", "name"],
  data() {
    this.extend = {
      "xAxis.0.axisLabel.rotate": 50
    };
    return {
      chartCpu: {
        columns: ["Time", "CPU", "Memory"],
        rows: []
      }
    };
  },
  methods: {
    async getStats() {
      await this.axios.get(`v1/container/add/stats/${this.id}`);
    },
    toTime(dateTime) {
      return dateFormat(dateTime, "MM:ss");
    }
  },
  watch: {},
  async created() {
    connection.on("monitorstats", stats => {
      var cpu =
        ((stats.cpuStats.cpuUsage.totalUsage -
          stats.preCPUStats.cpuUsage.totalUsage) /
          (stats.cpuStats.systemUsage - stats.preCPUStats.systemUsage)) *
        100;
      var memory = (stats.memoryStats.usage / stats.memoryStats.limit) * 100;
      this.chartCpu.rows.push({
        Time: this.toTime(stats.read),
        CPU: cpu.toFixed(2),
        Memory: memory.toFixed(2)
      });
      window.console.log(this.chartCpu.rows.length);
    });
    await this.getStats();
  },
  destroyed() {
    connection.methods["monitorstats"] = null; // 解除监控
  }
};
</script>