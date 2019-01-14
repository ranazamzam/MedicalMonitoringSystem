import { Component, Input, OnInit } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { Observable } from "rxjs";
import { SignalRConnectionInfo } from "../shared/models/signalr-connection-info.model";
import { Event } from "../shared/models/event.model";
import { AppointmentEvent } from "../shared/models/appointment-event.model";
import { AzureSignalRService } from '../azure-signal-r.service';

@Component({
  selector: 'app-conflict-display',
  templateUrl: './conflict-display.component.html',
  styleUrls: ['./conflict-display.component.css']
})
export class ConflictDisplayComponent implements OnInit {

  @Input() generatedEvents;

  private readonly _signalRService: AzureSignalRService;
  appointmentEventsWithConflicts: AppointmentEvent[] = [];
  p: number = 1;
  toggle: any = {};

  constructor(http: HttpClient, signalRService: AzureSignalRService) {
    this._signalRService = signalRService;
    this.toggle = this.appointmentEventsWithConflicts.map(i => false);
  }

  ngOnInit() {

    let that = this;
    //this._signalRService.init();
    this._signalRService.appointmentEventsWithConflicts.subscribe(data => {
      debugger;
      data.forEach(function (item) {

        const indexOfOriginalGeneratedEvent = that.generatedEvents.findIndex(x => x.eventId === item.EventId);

        if (indexOfOriginalGeneratedEvent != -1) {
          item.DoctorName = that.generatedEvents[indexOfOriginalGeneratedEvent].doctorName;
          item.EventDate = that.generatedEvents[indexOfOriginalGeneratedEvent].eventDateStr;
        }

        item.ConflictedEvents.forEach(conflictedEvent => {
          const indexOfConflictedGeneratedEvent = that.generatedEvents.findIndex(x => x.eventId === conflictedEvent.EventId);

          if (indexOfConflictedGeneratedEvent != -1) {
            conflictedEvent.PatientName = that.generatedEvents[indexOfConflictedGeneratedEvent].patientName;
          }

        })
      });

      this.appointmentEventsWithConflicts = this.appointmentEventsWithConflicts.concat(data);
    });
  }

}
