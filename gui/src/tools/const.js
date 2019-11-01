import {
    machineIdSync
} from "node-machine-id";

export function getToken() {
    try {
        return machineIdSync({
            original: true
        });
    } catch {
        window.console.log("获取token失败, 使用默认token-->98912984-c4e9-5ceb-8000-03882a0485e4")
        return "98912984-c4e9-5ceb-8000-03882a0485e4";
    }
}