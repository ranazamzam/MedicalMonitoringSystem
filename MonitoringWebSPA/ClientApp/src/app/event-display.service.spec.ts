import { TestBed, inject } from '@angular/core/testing';

import { EventDisplayService } from './event-display.service';

describe('EventDisplayService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [EventDisplayService]
    });
  });

  it('should be created', inject([EventDisplayService], (service: EventDisplayService) => {
    expect(service).toBeTruthy();
  }));
});
