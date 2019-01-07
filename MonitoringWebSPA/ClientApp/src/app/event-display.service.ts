import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { map, catchError } from 'rxjs/operators';
import { Config } from "./Config";

@Injectable()
export class EventDisplayService {

  private readonly _http: HttpClient;
  private readonly _baseUrl: string = Config.Urls.APIGateway;
  private patientUrl: string = '';
  private doctorUrl: string = '';
  private eventsUrl: string = '';

  constructor(http: HttpClient) {
    this._http = http;
    this.patientUrl = this._baseUrl + '/Patients';
    this.doctorUrl = this._baseUrl + '/Doctors';
    this.eventsUrl = Config.Urls.APIGatewayEvents;
  }

  getPatients() {
    debugger;
    return this._http.get(this.patientUrl).pipe(
      map((response: Response) => {
        debugger;
        return response;
      })
    );
  }

  getDoctors() {
    debugger;
    return this._http.get(this.doctorUrl).pipe(
      map((response: Response) => {
        debugger;
        return response;
      })
    );
  }

  getEvents() {
    debugger;
    return this._http.get(this.eventsUrl).pipe(
      map((response: any[]) => {
        debugger;
        return response;
      })
    );
  }
}
