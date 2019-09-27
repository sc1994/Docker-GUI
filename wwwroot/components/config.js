// 添加请求拦截器
axios.interceptors.request.use(config => {
    vm.loading++;
    return config;
}, error => {
    // 对请求错误做些什么
    console.error(error)
    vm.loading--;
    return Promise.reject(error);
});

// 添加响应拦截器
axios.interceptors.response.use(function (response) {
    vm.loading--;
    return response;
}, function (error) {
    // 对响应错误做点什么
    console.error(error)
    vm.loading--;
    return Promise.reject(error);
});