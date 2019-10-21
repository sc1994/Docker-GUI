<template>
  <page-header :name="name" :id="id" type="stats">
    <el-row>
      <el-col :span="24">
        <el-card class="chart" shadow="hover">
          <div slot="header" class="clearfix">
            <span>CPU per ({{current.cpu}}%)</span>
          </div>
          <ve-line
            :tooltip-visible="false"
            :height="height"
            :data="chartCpu"
            :extend="extend"
            :legend-visible="false"
            :settings="chartSettings"
            :colors="['#749f83']"
          ></ve-line>
        </el-card>
      </el-col>
      <el-col :span="12"></el-col>
    </el-row>
    <br />
    <el-row :gutter="10">
      <el-col :span="12">
        <el-card class="chart" shadow="hover">
          <div slot="header" class="clearfix">
            <span>Memory per ({{current.memory}}%)</span>
          </div>
          <ve-line
            :tooltip-visible="false"
            :data="chartMemory"
            :extend="extend"
            :legend-visible="false"
            :height="height"
            :colors="['#012345']"
          ></ve-line>
        </el-card>
      </el-col>
      <el-col :span="12">
        <el-card class="chart" shadow="hover">
          <div slot="header" class="clearfix">
            <span>Memory Value ({{current.memoryValue}}MB) -- limit {{current.maxMemory}}GB</span>
          </div>
          <ve-line
            :tooltip-visible="false"
            :data="chartMemoryValue"
            :extend="extend"
            :legend-visible="false"
            :height="height"
            :colors="['#c23531']"
          ></ve-line>
        </el-card>
      </el-col>
    </el-row>
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
      chartSettings: {
        // area: true
      },
      chartCpu: {
        columns: ["Time", "CPU"],
        rows: []
      },
      chartMemory: {
        columns: ["Time", "Memory"],
        rows: []
      },
      chartMemoryValue: {
        columns: ["Time", "MemoryValue"],
        rows: []
      },
      current: {
        cpu: 0,
        memory: 0,
        memoryValue: 0,
        maxMemory: 0
      },
      height: "360px"
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
    window.console.log("on monitorstats");
    connection.on("monitorstats", stats => {
      this.current.cpu = (
        ((stats.cpuStats.cpuUsage.totalUsage -
          stats.preCPUStats.cpuUsage.totalUsage) /
          (stats.cpuStats.systemUsage - stats.preCPUStats.systemUsage)) *
        100
      ).toFixed(2);
      this.current.memory = (
        (stats.memoryStats.usage / stats.memoryStats.limit) *
        100
      ).toFixed(2);
      this.chartCpu.rows.push({
        Time: this.toTime(stats.read),
        CPU: this.current.cpu
      });
      this.chartMemory.rows.push({
        Time: this.toTime(stats.read),
        Memory: this.current.memory
      });
      this.current.memoryValue = (
        stats.memoryStats.usage /
        1024 /
        1024
      ).toFixed(2);
      this.chartMemoryValue.rows.push({
        Time: this.toTime(stats.read),
        MemoryValue: this.current.memoryValue
      });
      this.current.maxMemory = (
        stats.memoryStats.limit /
        1024 /
        1024 /
        1024
      ).toFixed(2);

      if (this.chartCpu.rows.length > 100) {
        this.chartCpu.rows.shift();
        this.chartMemory.rows.shift();
        this.chartMemoryValue.rows.shift();
      }
    });
    await this.getStats();
  },
  destroyed() {
    window.console.log("delete monitorstats");
    // delete connection.methods.monitorstats; // 解除监控
  }
};
</script>

<style >
.chart .el-card__body {
  padding: 0px;
}
</style>