Vue.component("container-panel", async resolve => {
    var html = await axios.get("components/container-panel/index.html")
    resolve({
        template: html.data,
        props: ["dialog-detail"],
        data() {
            return {
                containers: [],

            }
        },
        methods: {
            detail(data) {
                this.dialogDetail.show = true;
                this.dialogDetail.title = data.image;
                this.dialogDetail.content = JSON.stringify(data, null, 2);
            },
            async stop(data) {
                var res = await axios.get(`v1/container/stop/${data.id}`);
                if (res.data.result) {
                    this.$notify({
                        title: "Successful",
                        message: `${data.image} has stopped`,
                        type: "success"
                    });
                }
                this.containers = res.data.list;
            },
            async start(data) {
                var res = await axios.get(`v1/container/start/${data.id}`);
                if (res.data.result) {
                    this.$notify({
                        title: "Successful",
                        message: `${data.image} has started`,
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
        async created() {
            await this.get();
        }
    })
})