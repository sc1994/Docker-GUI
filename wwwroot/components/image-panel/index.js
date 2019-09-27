Vue.component("image-panel", async resolve => {
    var html = await axios.get("components/image-panel/index.html")
    resolve({
        template: html.data,
        props: ["dialog-detail"],
        data() {
            return {
                images: [],
                matchName: "",
                transientMatch: ""
            }
        },
        methods: {
            detail(tag, name) {
                this.dialogDetail.show = true;
                this.dialogDetail.title = `${name}:${tag.tag}`
                this.dialogDetail.content = JSON.stringify(tag, null, 2);
            },
            async search() {
                var url = "v1/image";
                if (this.matchName) {
                    url += `?match=${this.matchName}`;
                }
                var imagesRes = await axios.get(url);
                this.images = imagesRes.data;
            }
        },
        computed: {

        },
        watch: {
            "matchName"(val, old) {
                if (val.trim() == old.trim()) return;
                this.search();
            }
        },
        async created() {
            await this.search();
        }
    });
});