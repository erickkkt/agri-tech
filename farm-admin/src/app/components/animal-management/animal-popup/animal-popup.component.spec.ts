import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AnimalPopupComponent } from './animal-popup.component';

describe('AnimalPopupComponent', () => {
  let component: AnimalPopupComponent;
  let fixture: ComponentFixture<AnimalPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AnimalPopupComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AnimalPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
