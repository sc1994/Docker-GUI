import * as signalR from "@aspnet/signalr";
import {
    Notification
} from 'element-ui';
import {
    getToken
} from '../tools/const'

const MessageDuration = 5000;

const connection = new signalR.HubConnectionBuilder()
    .withUrl(`http://localhost:5000/pull?token=${getToken()}`)
    .configureLogging(signalR.LogLevel.Information)
    .build();

var reCount = 0;

connection.start().then(() => {
    window.console.log("connected");
});

async function start() {
    try {
        await connection.start()
        Notification({
            message: "socket reconnected",
            type: 'success',
            duration: MessageDuration
        })
    } catch (err) {
        Notification({
            message: `socket try ${++reCount} times to reconnect`,
            type: 'warning',
            duration: MessageDuration
        })
        setTimeout(async () => {
            await start();
        }, 1000);
    }
}

// 重新连接
connection.onclose(async () => {
    Notification({
        message: "socket onclose",
        type: 'info',
        duration: MessageDuration
    })
    await start();
});

connection.on("notification", (type, msg) => {
    Notification({
        message: msg,
        type: type,
        duration: MessageDuration
    })
});

export default connection;