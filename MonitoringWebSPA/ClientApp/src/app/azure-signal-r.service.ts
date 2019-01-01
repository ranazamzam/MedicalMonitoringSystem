import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { Observable } from "rxjs";
import { SignalRConnectionInfo } from "./shared/models/signalr-connection-info.model";
import { map } from "rxjs/operators";
import { Subject } from "rxjs";
import { AppointmentEvent } from "./shared/models/appointment-event.model";

@Injectable()
export class AzureSignalRService {

  private readonly _http: HttpClient;
  private readonly _baseUrl: string = "http://localhost:7071/api/";
  private hubConnection: HubConnection;
  appointmentEventsWithConflicts: Subject<AppointmentEvent[]> = new Subject();

  constructor(http: HttpClient) {
    this._http = http;
  }

  private getConnectionInfo(): Observable<SignalRConnectionInfo> {
    let requestUrl = `${this._baseUrl}negotiate`;
    return this._http.get<SignalRConnectionInfo>(requestUrl);
  }

  init() {
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
        this.appointmentEventsWithConflicts.next(data);
        //data.forEach(function (item) {
        //  this.appointmentEventsWithConflicts.next(item);
        //});

      });
    });
  }

}
