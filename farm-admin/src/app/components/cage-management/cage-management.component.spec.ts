import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CageManagementComponent } from './cage-management.component';

describe('CageManagementComponent', () => {
  let component: CageManagementComponent;
  let fixture: ComponentFixture<CageManagementComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [CageManagementComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CageManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
