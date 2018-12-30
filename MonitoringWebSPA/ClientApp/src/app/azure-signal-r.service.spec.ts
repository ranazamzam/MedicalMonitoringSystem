import { TestBed, inject } from '@angular/core/testing';

import { AzureSignalRService } from './azure-signal-r.service';

describe('AzureSignalRService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AzureSignalRService]
    });
  });

  it('should be created', inject([AzureSignalRService], (service: AzureSignalRService) => {
    expect(service).toBeTruthy();
  }));
});
