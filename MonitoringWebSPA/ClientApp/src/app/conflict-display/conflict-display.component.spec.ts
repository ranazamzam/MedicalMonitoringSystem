import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConflictDisplayComponent } from './conflict-display.component';

describe('ConflictDisplayComponent', () => {
  let component: ConflictDisplayComponent;
  let fixture: ComponentFixture<ConflictDisplayComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConflictDisplayComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConflictDisplayComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
