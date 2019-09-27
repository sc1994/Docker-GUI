Vue.component("image-panel", async resolve => {
    var html = await axios.get("components/image-panel/index.html")
    resolve({
        template: html.data,
        props: ["dialog-detail", "search-image"],
        data() {
            return {
                images: []
            }
        },
        methods: {
            detail(tag, name) {
                this.dialogDetail.show = true;
                this.dialogDetail.title = `${name}:${tag.tag}`
                this.dialogDetail.content = JSON.stringify(tag, null, 2);
            },
            async search() {
                if (this.searchImage.type != 1) return;
                var url = "v1/image";
                if (this.searchImage.key) {
                    url += `?match=${this.searchImage.key}`;
                }
                var imagesRes = await axios.get(url);
                this.images = imagesRes.data;
            }
        },
        computed: {

        },
        watch: {
            "searchImage.key"(val, old) {
                if (val.trim() == old.trim()) return;
                this.search();
            },
            "searchImage.handle"() {
                this.search();
            }
        },
        async created() {
            await this.search();
        }
    });
});