<template>
  <page-header :name="name" :id="id" type="stats">
    <el-row>
      <el-col :span="24">
        <el-card class="chart" shadow="hover">
          <ve-line :data="chartData" :settings="chartSettings"></ve-line>
        </el-card>
      </el-col>
      <el-col :span="12"></el-col>
    </el-row>
    <br />
    <el-row :gutter="10">
      <el-col :span="12">
        <el-card class="chart" shadow="hover">
          <div slot="header" class="clearfix">
            <span>Memory per (%)</span>
          </div>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card class="chart" shadow="hover">
          <div slot="header" class="clearfix">
            <span>Memory Value (MB) -- limit GB</span>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </page-header>
</template>

<script>
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
    this.chartSettings = {
      labelMap: {
        cpuPercent: "CPU per (%)",
        pids: "PID count",
        axisSite: { right: ["pids"] },
        yAxisType: ["percent", "KMB"],
        yAxisName: ["CPU per (%)", "PID count"]
      },
      xAxisType: "time"
    };
    return {
      chartData: {
        columns: ["time", "cpuPercent", "pids"],
        rows: []
      },
      height: "360px"
    };
  },
  methods: {
    async getStats() {
      var res = await this.axios.get(
        `sentry/${this.id}/2019-10-26 09:18/2019-10-27/stats`
      );
      this.chartData.rows = res.data;
    },
    toTime(dateTime) {
      return dateFormat(dateTime, "MM:ss");
    }
  },
  watch: {},
  async created() {
    await this.getStats();
    // setInterval(async () => {
    //   await this.getStats();
    // }, 5000);
  },
  destroyed() {}
};
</script>
