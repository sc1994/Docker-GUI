Vue.component("docker-hub", async resolve => {
    var html = await axios.get("components/docker-hub/index.html");
    resolve({
        template: html.data,
        props: ["search-image"],
        data() {
            return {
                list: []
            }
        },
        methods: {
            async search() {
                if (this.searchImage.key && this.searchImage.key.trim() && this.searchImage.type == 2) {
                    var res = await axios.get(`v1/image/search/${this.searchImage.key}`);
                    this.list = res.data;
                }
            }
        },
        watch: {
            "searchImage.handle"() {
                this.search();
            }
        },
        created() {

        },
    });
});