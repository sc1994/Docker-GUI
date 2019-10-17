<template>
  <div>
    <el-page-header @back.once="goBack" :content="name.replace('++', '/')"></el-page-header>
    <!-- <el-pagination
      :page-size="pageSize"
      :pager-count="pageCount"
      :current-page="pageCurrent"
      layout="prev, pager, next"
      :total="logCount"
      style="text-align: center;"
    ></el-pagination>-->
    <br />
    <el-col :span="23">
      <el-card shadow="never" class="console" ref="consoleDiv">
        <div v-for="log in showLogList" :key="log.key">{{log.log}}</div>
      </el-card>
    </el-col>
    <el-col :span="1">
      <el-slider
        v-model="pageCurrent"
        :step="pageSize"
        vertical
        height="91vh"
        :min="0"
        :max="logList.length"
        :marks="sliderMark"
      ></el-slider>
    </el-col>
  </div>
</template>

<script>
import connection from "../plugins/signalR";

export default {
  props: ["id", "name"],
  data() {
    return {
      logList: [],
      showLogList: [],
      pageSize: 45,
      pageCurrent: 0
    };
  },
  computed: {
    sliderMark() {
      let r = {};
      r["1"] = "1";
      for (let index = 1; index < this.logList.length / 200; index++) {
        let l = index * 200 + "";
        r[l] = l;
      }
      if (!r[this.logList.length + ""]) {
        r[this.logList.length + ""] = this.logList.length + "";
      }
      return r;
    }
  },
  methods: {
    async goBack() {
      await this.axios.get(`v1/container/cancel/log/${this.id}`);
      this.$router.push("/");
    },
    async getLog() {
      await this.axios.get(`v1/container/add/log/${this.id}`);
    },
    pushLog(log) {
      this.logList.push({
        key: this.logList.length,
        log
      });
      if (this.logList.length % 10 == 0 && this.pageCurrent == 0) {
        this.showLogList = this.logList.slice(
          this.logList.length - this.pageSize,
          this.logList.length
        );
      }
    }
  },
  watch: {
    pageCurrent(val) {
      let s = this.logList.length - val - this.pageSize;
      if (s < 0) {
        s = 0;
      }
      let e = this.logList.length - val;
      this.showLogList = this.logList.slice(s, e);
    }
  },
  async created() {
    connection.on("monitorLog", log => {
      this.pushLog(log);
    });
    await this.getLog();
    document.addEventListener("scroll", this.onScroll);
  }
};
</script>

<style >
.console {
  white-space: pre;
  word-break: keep-all;
  background-color: #012345;
  color: #ccc;
  font-family: "Courier New", Courier, monospace;
  height: 91vh;
  overflow: auto;
}
</style>