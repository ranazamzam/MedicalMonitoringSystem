import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { map, catchError } from 'rxjs/operators';

@Injectable()
export class EventDisplayService {

  private readonly _http: HttpClient;
  private readonly _baseUrl: string = "http://localhost:19081/MedicalBookingSystem/MedicalBookingSystem.APIGateway/";
  private patientUrl: string = '';
  private doctorUrl: string = '';

  constructor(http: HttpClient) {
    this._http = http;
    this.patientUrl = this._baseUrl + '/Patients';
    this.doctorUrl = this._baseUrl + '/Doctors';
  }

  getPatients() {
    debugger;
    return this._http.get(this.patientUrl).pipe(
      map((response: Response) => {
        debugger;
        return response;
      })
    );
     // .map((response) => {
    //  let data;
      //if (response.status == 200)
      //  data = response.json();
      //else
      //  console.log('No data found');

      //return data;
  //  });
  }
}
