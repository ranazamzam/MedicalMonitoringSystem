import { Event } from './event.model';


export class AppointmentEvent extends Event{
  ConflictedEvents: Event[];
}
