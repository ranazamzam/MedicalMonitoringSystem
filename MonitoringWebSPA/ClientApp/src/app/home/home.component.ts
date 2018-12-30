import { Component, OnInit } from '@angular/core';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { Observable, Subject } from 'rxjs';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
 
  private _hubConnection: HubConnection | undefined;
  public async: any;
  p: number = 1;
  generatedEvents: any[] = [];

  ngOnInit() {

    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:19081/MedicalBookingSystem/MedicalBookingSystem.APIGateway/eventsGenerator')
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this._hubConnection.start().catch(err => console.error(err.toString()));

    this._hubConnection.on('ReceiveNewEvent', (data: any) => {
      debugger;
      //const received = `Received: ${data}`;
      this.generatedEvents.push(data);
    });
  }

}
