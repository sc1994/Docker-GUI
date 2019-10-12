Vue.component("container-panel", async resolve => {
    var html = await axios.get("components/container-panel/index.html")
    resolve({
        template: html.data,
        props: ["dialog-detail"],
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
                    list: [],
                }
            }
        },
        methods: {
            detail(data) {
                this.dialogDetail.show = true;
                this.dialogDetail.title = data.image;
                this.dialogDetail.content = JSON.stringify(data, null, 2);
            },
            monitor(type, data) {
                axios.get(`v1/container/add/${type}/${data.id}`);
                this.current = {
                    type,
                    data
                };
                vm.loading--; // 取消加载动画
                this.dialogDetail.title = data.names.join(";");
                this.dialogDetail.show = true;
            },
            async cancelMonitor() {
                var res = await axios.get(`v1/container/cancel/${this.current.type}/${this.current.data.id}`);
                console.log(res);
            },
            async setStatus(type, data) {
                var res = await axios.get(`v1/container/${type}/${data.id}`);
                if (res.data.result) {
                    this.$notify({
                        title: "Successful",
                        message: `${data.image} has ${type}ed`,
                        type: "success"
                    });
                }
                this.containers = res.data.list;
            },
            async get() {
                let list = await axios.get("/v1/container");
                this.containers = list.data;
            },
            showPorts(ports) {
                if (!ports || ports.length < 1) return "";
                let r = "";
                for (let item of ports) {
                    if (item.ip) {
                        r += item.ip + ":"
                    }
                    r += item.privatePort;
                    if (item.publicPort) {
                        r += "->" + item.publicPort
                    }
                    if (item.type) {
                        r += "/" + item.type;
                    }
                    if (ports.indexOf(item) != ports.length - 1) {
                        r += "; "
                    }
                }
                return r;
            },
            showStatus(status, type) {
                var a = status.split(" ");
                if (type == 0) { // 取头
                    let r = a[0];
                    if (r == "Exited") {
                        return {
                            0: "Exited",
                            1: "Code:" + a[1]
                        }
                    }
                    return {
                        0: r
                    }
                } else { // 取尾
                    return {
                        0: a[a.length - 2] + " " + a[a.length - 1]
                    }
                }
            }
        },
        watch: {
            async "dialogDetail.show"(val) {
                if (!val && this.current) { // 兼容
                    await this.cancelMonitor();
                    this.dialogDetail.title = "";
                    this.dialogDetail.content = "";
                }
            }
        },
        async created() {
            await this.get();
            // 接受监控数据
            connection.on("monitor", (type, message) => {
                if (type == "stats") {
                    // this.statsList.push(message); // TODO:未来做可视化
                    // if (this.statsList.length > 100) {
                    //     this.statsList.splice(1, 1) // 删除第一个
                    // }
                    this.dialogDetail.content = JSON.stringify(message, null, 2);
                } else if (type == "log") {
                    this.logList.push(message);
                    console.log(this.logList.length);
                    // // content = this.logList.join("\r\n");
                    // let t = this.dialogDetail.content,
                    //     l = this.dialogDetail.content.length,
                    //     m = 20000;

                    // if (l > m) {
                    //     t = this.dialogDetail.content.substring(this.dialogDetail.content.length - m);
                    // }
                    // this.dialogDetail.content = t + "\r\n" + message
                    // let txt = document.getElementsByTagName("textarea")[0];
                    // txt.scrollTop = txt.scrollHeight;
                }
            })
            // 取消监控的后续操作
            connection.on("cancelMonitor", (type, message) => {
                console.log(type, message);
                if (type == "stats") {
                    this.statsList = []; // 清空统计数据
                } else if (type == "log") {
                    this.logList = []; // 清空log数据
                }
                this.current = {}; // 清空当前选择的数据
            })
        }
    })
})