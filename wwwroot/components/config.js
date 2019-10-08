// 添加请求拦截器
axios.interceptors.request.use(config => {
    vm.loading++;
    if (connection && connection.connectionId) {
        if (!config.headers) config.headers = {};
        config.headers.common["connectionId"] = connection.connectionId;
    }
    return config;
}, error => {
    // 对请求错误做些什么
    if (vm.loading > 0)
        vm.loading--;
    return Promise.reject(error);
});

// 添加响应拦截器
axios.interceptors.response.use(function (response) {
    if (vm.loading > 0)
        vm.loading--;
    return response;
}, function (error) {
    // 对响应错误做点什么
    if (vm.loading > 0)
        vm.loading--;
    return Promise.reject(error);
});

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/pull")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(function () {
    setInterval(() => {
        connection.invoke("ping").catch(err => console.error(err.toString()));
    }, 20000)
});

connection.on("pong", (message) => {
    console.log(message);
})

connection.on("pull", (message) => {
    console.log(message);
    // TODO:下载中心
})

connection.on("error", (ex) => {
    console.warn(ex);
    vm.$message.error(ex.message);
})