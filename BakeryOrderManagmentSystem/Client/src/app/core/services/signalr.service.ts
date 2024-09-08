import { Injectable } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { environment } from '../../../environments/environments.development';

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    private hubConnection: HubConnection;
    private messageSubject = new Subject<string>();
    public message$ = this.messageSubject.asObservable();
    private apiUrl = `${environment.apiUrl}`;

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
        .withUrl(`${this.apiUrl}/notificationHub`, {
            skipNegotiation: true,
            transport: HttpTransportType.WebSockets
        })
        .configureLogging(LogLevel.Information) 
        .withAutomaticReconnect()
        .build();

    this.hubConnection.on('ReceiveProductDeleted', (productId: number) => {
        this.messageSubject.next(`Product with ID ${productId} was deleted.`);
    });

    this.hubConnection.start()
        .catch(err => console.error('Error while starting SignalR connection: ', err));
  }
}