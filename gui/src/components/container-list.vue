<template>
  <div>
    <el-row>
      <el-col :span="2">Status</el-col>
      <el-col :span="4">Image</el-col>
      <el-col :span="5">Command</el-col>
      <el-col :span="4">Created</el-col>
      <el-col :span="7">Ports</el-col>
    </el-row>
    <hr style="border-style: dashed;" />
    <el-row v-for="container in containers" :key="container.id" class="line-text">
      <el-col :span="2">
        <el-tooltip :content="container.status" placement="right">
          <el-tag v-if="container.state=='running'" size="small ">
            <i class="el-icon-sunny"></i>
            {{container.state}}
          </el-tag>
          <el-tag type="danger" v-else size="small ">
            <i class="el-icon-heavy-rain"></i>
            {{container.state}}
          </el-tag>
        </el-tooltip>
      </el-col>
      <el-col :span="4" :title="container.image">
        <i class="el-icon-cpu"></i>
        {{container.image}}
      </el-col>
      <el-col :span="5" :title="container.command">{{container.command}}</el-col>
      <el-col :span="4">{{container.createdStr}}</el-col>
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
      }
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
          r += "->" + item.publicPort;
        }
        if (item.type) {
          r += "/" + item.type;
        }
        if (ports.indexOf(item) != ports.length - 1) {
          r += "; ";
        }
      }
      return r;
    }
  },
  async created() {
    var list = await this.axios.get("v1/container");
    this.containers = list.data;
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
</style>