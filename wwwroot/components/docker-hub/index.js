Vue.component("docker-hub", async resolve => {
    var html = await axios.get("components/docker-hub/index.html");
    resolve({
        template: html.data,
        props: ["search-image"],
        data() {
            return {
                list: [],
                tag: {
                    show: false,
                    current: "",
                    list: [],
                    count: 0
                },
                openTags: false
            }
        },
        methods: {
            async search() {
                if (this.searchImage.key && this.searchImage.key.trim() && this.searchImage.type == 2) {
                    var res = await axios.get(`v1/image/search/${this.searchImage.key}`);
                    this.list = res.data;
                }
            },
            async getTags(data) { // todo:远程响应太慢,不好调
                var temp = ++this.tag.count;
                this.tag.current = data.name;
                this.tag.show = true;
                var res = await axios.get(`v1/image/search/tags?image=${this.tag.current}`);
                if (temp == this.tag.count) {
                    this.tag.list = res.data;
                } else {
                    console.warn("错误的数据列表", this.tag, res.data);
                }
            },
            async pull(tag) {
                var res = await axios.post(`v1/image/pull`, {
                    fromImage: this.tag.current,
                    tag: tag
                })
                console.log(res);
            }
        },
        watch: {
            "searchImage.handle"() {
                if (this.searchImage.type != 2) return;
                this.search();
            }
        },
        async created() {

        },
    });
});