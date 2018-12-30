import { Component, OnInit } from '@angular/core';
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';

@Component({
  selector: 'app-conflict-display',
  templateUrl: './conflict-display.component.html',
  styleUrls: ['./conflict-display.component.css']
})
export class ConflictDisplayComponent implements OnInit {

  private readonly _http: HttpClient;
  private readonly _baseUrl: string = "http://localhost:7071/api/";
  private hubConnection: HubConnection;

  constructor(http: HttpClient) {
    this._http = http;
  }

  private getConnectionInfo(): Observable<SignalRConnectionInfo> {
    let requestUrl = `${this._baseUrl}negotiate`;
    return this._http.get<SignalRConnectionInfo>(requestUrl);
  }

  ngOnInit() {
    this.getConnectionInfo().subscribe(info => {

      info.accessToken = info.accessToken || info.accessKey;
      info.url = info.url || info.endpoint;

      let options = {
        accessTokenFactory: () => info.accessKey
      };

      this.hubConnection = new signalR.HubConnectionBuilder()
        .withUrl(info.url, options)
        .configureLogging(signalR.LogLevel.Information)
        .build();

      this.hubConnection.start().catch(err => console.error(err.toString()));

      this.hubConnection.on('notify', (data: any) => {

      });
    });
  }

}
