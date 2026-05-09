import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CagePopupComponent } from './cage-popup.component';

describe('CagePopupComponent', () => {
  let component: CagePopupComponent;
  let fixture: ComponentFixture<CagePopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CagePopupComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CagePopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
