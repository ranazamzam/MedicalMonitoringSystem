import { Component, OnInit } from '@angular/core';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { Observable, Subject } from 'rxjs';
import { AzureSignalRService } from '../azure-signal-r.service';
import { EventDisplayService } from '../event-display.service';
import { indexDebugNode } from '@angular/core/src/debug/debug_node';
import { Config } from "../Config";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  private readonly _signalRService: AzureSignalRService;
  private readonly _eventsDisplayervice: EventDisplayService;
  private _hubConnection: HubConnection | undefined;
  public async: any;
  p: number = 1;
  generatedEvents: any[] = [];
  filteredGeneratedEvents: any[] = [];
  patients: any = null;
  doctors: any = null;
  selectedPatientId: number = -1;
  selectedDoctorId: number = -1;

  constructor(signalRService: AzureSignalRService, eventsDisplayervice: EventDisplayService) {
    this._signalRService = signalRService;
    this._eventsDisplayervice = eventsDisplayervice;
  }

  ngOnInit() {

    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(Config.Urls.APIGatewaySignalR)
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this._hubConnection.start().catch(err => console.error(err.toString()));

    this._hubConnection.on('ReceiveNewEvent', (data: any) => {
      debugger;
      this.generatedEvents.push(data);
      this.applyFilter({ patientId: this.selectedPatientId, doctorId: this.selectedDoctorId });
    });

    this._signalRService.init();
    this._signalRService.appointmentEventsWithConflicts.subscribe(data => {
      debugger;
      let that = this;
      data.forEach(function (item) {
        item.ConflictedEvents.forEach(conflictedEvent => {
          const indexOfGeneratedEvent = that.generatedEvents.findIndex(x => x.eventId === conflictedEvent.EventId);
          const indexOfFilteredGeneratedEvent = that.filteredGeneratedEvents.findIndex(x => x.eventId === conflictedEvent.EventId);

          if (indexOfGeneratedEvent != -1) {
            that.generatedEvents[indexOfGeneratedEvent].isConflictShown = true;
          }

          if (indexOfFilteredGeneratedEvent != -1) {
            that.filteredGeneratedEvents[indexOfFilteredGeneratedEvent].isConflictShown = true;
          }

        })
      });
    });

    this._eventsDisplayervice.getPatients().subscribe((patients) => {
      debugger;
      this.patients = patients;
    });

    this._eventsDisplayervice.getDoctors().subscribe((doctors) => {
      debugger;
      this.doctors = doctors;
    });

    this._eventsDisplayervice.getEvents().subscribe((events) => {
      debugger;

      events.forEach(event => {
        this.generatedEvents.push(event);
      });

      this.applyFilter({ patientId: this.selectedPatientId, doctorId: this.selectedDoctorId });
    });
  }

  applyFilter(data: any) {
    debugger;
    //var duplicateObject = JSON.parse(JSON.stringify(originalObject));
    this.filteredGeneratedEvents = [];
    this.filteredGeneratedEvents = this.filteredGeneratedEvents.concat(this.generatedEvents);

    if (data != null && data != undefined) {

      this.selectedDoctorId = data.doctorId;
      this.selectedPatientId = data.patientId;

      if (data.patientId != undefined && data.patientId != -1) {
        this.filteredGeneratedEvents = this.generatedEvents.filter(event => event.patientId == data.patientId);
      }

      if (data.doctorId != undefined && data.doctorId != -1) {
        this.filteredGeneratedEvents = this.generatedEvents.filter(event => event.doctorId == data.doctorId);
      }
    }
  }
}
