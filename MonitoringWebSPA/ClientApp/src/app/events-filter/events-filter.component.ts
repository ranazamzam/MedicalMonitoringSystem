import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';

@Component({
  selector: 'app-events-filter',
  templateUrl: './events-filter.component.html',
  styleUrls: ['./events-filter.component.css']
})
export class EventsFilterComponent implements OnInit {

  @Input() patients;
  @Input() doctors;

  private selectedPatientId: number = -1;
  private selectedPatientName: string = 'All';

  private selectedDoctorId: number = -1;
  private selectedDoctorName: string = 'All';

  @Output() onFilterValueChanged: EventEmitter<any> = new EventEmitter<any>();

  constructor() { }

  ngOnInit() {
  }

  patientFilterChanged(patient: any) {
    this.selectedPatientId = patient.id;
    this.selectedPatientName = patient.name;
    this.onFilterValueChanged.emit({ patientId: this.selectedPatientId, doctorId: this.selectedDoctorId });
  }

  doctorFilterChanged(doctor: any) {
    this.selectedDoctorId = doctor.id;
    this.selectedDoctorName = doctor.name;
    this.onFilterValueChanged.emit({ patientId: this.selectedPatientId, doctorId: this.selectedDoctorId });
  }

}
