import { Component, OnInit } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { Observable } from "rxjs";
import { SignalRConnectionInfo } from "../shared/models/signalr-connection-info.model";
import { Event } from "../shared/models/event.model";
import { AppointmentEvent } from "../shared/models/appointment-event.model";

@Component({
  selector: 'app-conflict-display',
  templateUrl: './conflict-display.component.html',
  styleUrls: ['./conflict-display.component.css']
})
export class ConflictDisplayComponent implements OnInit {

  private readonly _http: HttpClient;
  private readonly _baseUrl: string = "http://localhost:7071/api/";
  private hubConnection: HubConnection;
  appointmentEventsWithConflicts: AppointmentEvent[] = [];
  p: number = 1;
  toggle: any = {};

  constructor(http: HttpClient) {
    this._http = http;
    this.toggle = this.appointmentEventsWithConflicts.map(i => false);
  }

  private getConnectionInfo(): Observable<SignalRConnectionInfo> {
    let requestUrl = `${this._baseUrl}negotiate`;
    return this._http.get<SignalRConnectionInfo>(requestUrl);
  }

  ngOnInit() {
    this.getConnectionInfo().subscribe(info => {

      info.accessToken = info.accessToken;
      info.url = info.url;

      let options = {
        accessTokenFactory: () => info.accessToken
      };

      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(info.url, options)
        .configureLogging(signalR.LogLevel.Information)
        .build();

      this.hubConnection.start().catch(err => console.error(err.toString()));

      this.hubConnection.on('appointmentConflictDetected', (data: AppointmentEvent[]) => {
        debugger;
        this.appointmentEventsWithConflicts= this.appointmentEventsWithConflicts.concat(data);
        // loop on the incoming conflicted events list and group it by the original event reference number
        //data.forEach(function (item) {

        //  this.appointmentEvents[item.eventId] = this.appointmentEvents[item.eventId] || [];
        //  this.appointmentEvents[item.eventId].conflictedEvents.push({ item });
        //});

      });
    });
  }

}
