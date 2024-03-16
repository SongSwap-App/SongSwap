import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
    .withUrl('hub') // Specify the URL of your SignalR hub
    .build();

connection.start().catch(err => console.error('SignalR connection error: ', err));

export default connection;
