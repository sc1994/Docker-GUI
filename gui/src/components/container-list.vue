<template>
  <div v-loading="loading" style="height:100%">
    <el-row>
      <el-col :span="2">Status</el-col>
      <el-col :span="4">Image</el-col>
      <el-col :span="5">Command</el-col>
      <el-col :span="3">Created</el-col>
      <el-col :span="7">Ports</el-col>
      <el-col :span="2">
        <i class="el-icon-view breathe-div"></i>
      </el-col>
      <!-- TODO: 展示容器大小 -->
    </el-row>
    <hr style="border-style: dashed;" />
    <el-row v-for="container in containers" :key="container.id" class="line-text">
      <el-col :span="2">
        <el-tooltip :content="container.status" placement="right">
          <el-tag v-if="container.state=='running'" size="small ">{{container.state}}</el-tag>
          <el-tag type="danger" v-else size="small ">{{container.state}}</el-tag>
        </el-tooltip>
      </el-col>
      <el-col :span="4" :title="container.image">
        <i class="el-icon-cpu"></i>
        {{container.image}}
      </el-col>
      <el-col :span="5" :title="container.command">{{container.command}}</el-col>
      <el-col :span="3">{{container.createdStr}}</el-col>
      <el-col :span="7" :title="showPorts(container.ports)">{{showPorts(container.ports)}}&nbsp;</el-col>
      <el-col :span="2">
        <el-dropdown trigger="click">
          <el-button type="text" icon="el-icon-setting" style="padding: 0px;"></el-button>
          <el-dropdown-menu slot="dropdown">
            <el-dropdown-item>
              <el-button type="text" @click="detail(container)">Detail</el-button>
            </el-dropdown-item>
            <el-dropdown-item>
              <el-button type="text" @click="monitor('stats', container)">Stats</el-button>
            </el-dropdown-item>
            <el-dropdown-item>
              <el-button type="text" @click="monitor('log', container)">Log</el-button>
            </el-dropdown-item>
            <el-dropdown-item v-if="container.state=='running'">
              <el-button type="text" @click="setStatus('stop', container)">Stop</el-button>
            </el-dropdown-item>
            <el-dropdown-item v-else>
              <el-button type="text" @click="setStatus('start', container)">Start</el-button>
            </el-dropdown-item>
          </el-dropdown-menu>
        </el-dropdown>
      </el-col>
    </el-row>
  </div>
</template>

<script>
export default {
  data() {
    return {
      containers: [],
      current: {},
      statsList: [],
      logList: [],
      dialogLog: {
        title: "",
        count: 0,
        show: false,
        list: []
      },
      loading: false
    };
  },
  methods: {
    showPorts(ports) {
      if (!ports || ports.length < 1) return "";
      let r = "";
      for (let item of ports) {
        if (item.ip) {
          r += item.ip + ":";
        }
        r += item.privatePort;
        if (item.publicPort) {
          r += " -> " + item.publicPort;
        }
        if (item.type) {
          r += "/" + item.type;
        }
        if (ports.indexOf(item) != ports.length - 1) {
          r += "; ";
        }
      }
      return r;
    },
    monitor(type, container) {
      var encode = container.image.replace(/\//g, "++");
      let toUrl = `/${type}/${container.id}/${encode}`;
      this.$router.push(toUrl);
    }
  },
  async created() {
    this.loading = true;
    var list = await this.axios.get("v1/container");
    this.containers = list.data;
    this.loading = false;
  }
};
</script>

<style scoped>
.line-text {
  font-size: 13px;
  color: #303133;
  line-height: 260%;
}

.line-text .el-col {
  text-overflow: ellipsis;
  white-space: nowrap;
  overflow: hidden;
}

.breathe-div {
  /* width: 100px;
  height: 100px; */
  border: 1px solid #fafafa;
  border-radius: 50%;
  text-align: center;
  cursor: pointer;
  /* margin: 150px auto; */
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
  overflow: hidden;
  -webkit-animation-timing-function: ease-in-out;
  -webkit-animation-name: breathe;
  -webkit-animation-duration: 1500ms;
  -webkit-animation-iteration-count: infinite;
  -webkit-animation-direction: alternate;
}

@-webkit-keyframes breathe {
  0% {
    opacity: 0.4;
    box-shadow: 0 1px 2px rgba(0, 147, 223, 0.4),
      0 1px 1px rgba(0, 147, 223, 0.1) inset;
  }

  100% {
    opacity: 1;
    border: 1px solid rgba(59, 235, 235, 0.7);
    box-shadow: 0 1px 20px #0093df, 0 1px 20px #0093df inset;
  }
}
</style>