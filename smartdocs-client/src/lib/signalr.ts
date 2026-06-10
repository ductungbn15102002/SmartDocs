import * as signalR from '@microsoft/signalr'

export const connection = new signalR.HubConnectionBuilder()
  .withUrl('https://localhost:7007/hubs/document', {
    accessTokenFactory: () => localStorage.getItem('accessToken') || ''
  })
  .withAutomaticReconnect()
  .build()

export const startConnection = async () => {
  if (connection.state === signalR.HubConnectionState.Disconnected) {
    await connection.start()
  }
}