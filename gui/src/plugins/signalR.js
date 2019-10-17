import * as signalR from "@aspnet/signalr";
import {
    Message
} from 'element-ui'
import {
    getToken
} from '../tools/const'

const MessageDuration = 8000;

let connection = new signalR.HubConnectionBuilder()
    .withUrl(`http://localhost:5000/pull?token=${getToken()}`)
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().then(() => {
    window.console.log("connected");
});

connection.on("error", ex => {
    Message({
        message: ex,
        type: 'error',
        duration: MessageDuration
    })
});

connection.on("cancelMonitor", (_, m) => {
    Message({
        message: m,
        type: 'info',
        duration: MessageDuration
    })
});

export default connection;