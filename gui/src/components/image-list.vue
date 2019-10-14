<template>
  <el-collapse accordion>
    <el-collapse-item v-for="image in images" :key="image.repository">
      <template slot="title">
        <i class="el-icon-collection" style="margin-right: 6px;"></i>
        {{image.repository}}
      </template>
      <el-row>
        <el-col :span="5">Tag</el-col>
        <el-col :span="7">Id</el-col>
        <el-col :span="4">Size</el-col>
        <el-col :span="6">Created</el-col>
        <el-col :span="2"></el-col>
      </el-row>
      <hr style="border-style: dashed;" />
      <el-row v-for="tag in image.tags" :key="tag.tag">
        <el-col :span="5">
          <i class="el-icon-collection-tag" style="margin-right: 4px;"></i>
          {{tag.tag}}
        </el-col>
        <el-col :span="7">{{tag.imageId.split(":")[1].substring(0,12)}}</el-col>
        <el-col :span="4">
          <span v-if="tag.size >= 1000000000">{{(tag.size / 1000000000).toFixed(2)}}G</span>
          <span v-else>{{(tag.size / 1000000).toFixed(2)}}M</span>
        </el-col>
        <el-col :span="6">{{tag.created}}</el-col>
        <el-col :span="2">
          <el-dropdown trigger="click">
            <el-button type="text" icon="el-icon-setting" style="padding: 0px;"></el-button>
            <el-dropdown-menu slot="dropdown">
              <el-dropdown-item>
                <el-button type="text" @click="detail(tag, image.repository)">Detail</el-button>
              </el-dropdown-item>
              <el-dropdown-item>
                <el-button type="text" v-on:click.stop>Inspect</el-button>
              </el-dropdown-item>
              <el-dropdown-item>
                <el-button type="text" v-on:click.stop>History</el-button>
              </el-dropdown-item>
              <el-dropdown-item>
                <el-button type="text" v-on:click.stop>Push</el-button>
              </el-dropdown-item>
              <el-dropdown-item>
                <el-button type="text" v-on:click.stop>Tag</el-button>
              </el-dropdown-item>
              <el-dropdown-item>
                <el-button type="text" v-on:click.stop>Remove</el-button>
              </el-dropdown-item>
            </el-dropdown-menu>
          </el-dropdown>
        </el-col>
      </el-row>
    </el-collapse-item>
  </el-collapse>
</template>

<script>
export default {
  props: ["searchRequest"],
  data() {
    return {
      images: []
    };
  },
  methods: {
    async search() {
      if (this.searchRequest.type !== 1) return;
      var url = "v1/image";
      if (this.searchRequest.key) {
        url += `?match=${this.searchRequest.key}`;
      }
      var imagesRes = await this.axios.get(url);
      this.images = imagesRes.data;
    }
  },
  watch: {
    "searchRequest.handle"() {
      this.search();
    }
  },
  created() {
    this.search();
  }
};
</script>
