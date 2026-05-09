import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FarmPopupComponent } from './farm-popup.component';

describe('FarmPopupComponent', () => {
  let component: FarmPopupComponent;
  let fixture: ComponentFixture<FarmPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FarmPopupComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FarmPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
