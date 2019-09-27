var vm = new Vue({
    el: "#app",
    data() {
        return {
            images: [],
            loading: 0,
            dialogDetail: {
                show: false,
                title: "",
                content: ""
            }
        }
    },
    methods: {
        async refreshImages() {
            var res = await axios.get("v1/image/refresh")
            if (res.data == true) {
                this.$notify({
                    title: 'Refresh the success',
                    type: 'success'
                });
            }
        }
    },
    watch: {
        loading(val) {
            if (val == 1) {
                loading = this.$loading({
                    lock: true,
                    text: 'Loading',
                    spinner: 'el-icon-loading',
                    background: 'rgba(0, 0, 0, 0.5)'
                });
            } else if (val == 0) {
                try {
                    loading.close();
                } catch (ex) {
                    console.warn(ex);
                }
            }
        }
    }
});