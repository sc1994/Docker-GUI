<template>
  <div>
    <el-row :gutter="15">
      <el-col :span="8">
        <el-card class="full-screen-card" shadow="hover">
          <el-input
            placeholder="Search Key"
            v-model="searchImage.key"
            class="input-with-select"
            style="margin-bottom: 10px;"
          >
            <el-select v-model="searchImage.type" slot="prepend">
              <el-option label="Local" :value="1"></el-option>
              <el-option label="Remote" :value="2"></el-option>
            </el-select>
            <el-button slot="append" icon="el-icon-search" @click="searchImage.handle++;"></el-button>
          </el-input>
          <el-collapse-transition>
            <image-list :searchRequest="searchImage" v-show="searchImage.type==1" />
          </el-collapse-transition>
          <!--   <transition name="el-zoom-in-bottom">
            <docker-hub v-show="searchImage.type==2" :search-image="searchImage"></docker-hub>
          </transition>-->
        </el-card>
      </el-col>
      <el-col :span="16">
        <el-card class="full-screen-card" shadow="hover">
          <container-list />
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script>
// @ is an alias to /src

import ContainerList from "@/components/container-list";
import ImageList from "@/components/image-list";

export default {
  components: {
    ContainerList,
    ImageList
  },
  data() {
    return {
      searchImage: {
        key: "",
        type: 1,
        handle: 1
      }
    };
  }
};
</script>

<style scoped>
.el-select {
  width: 105px;
}

.full-screen-card {
  height: 98vh;
  overflow: auto;
}

.el-card__body {
  padding: 20px;
  /* height: 100%; TODO:未生效 , 将会优化加载动画的位置居中*/
}
</style>
