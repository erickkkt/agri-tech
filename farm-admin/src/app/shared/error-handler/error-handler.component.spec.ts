import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ErrorHandlerComponent } from './error-handler.component';
import { Router,  ParamMap, Params, convertToParamMap } from '@angular/router';

import { ReplaySubject } from 'rxjs';

export class ActivatedRouteStub {
  // Use a ReplaySubject to share previous values with subscribers
  // and pump new values into the `paramMap` observable
  private subject = new ReplaySubject<ParamMap>();

  constructor(initialParams: Params) {
    this.setParamMap(initialParams);
  }

  /** The mock paramMap observable */
  readonly paramMap = this.subject.asObservable();

  /** Set the paramMap observables's next value */
  setParamMap(params: Params) {
    this.subject.next(convertToParamMap(params));
  };
}

describe('ErrorHandlerComponent', () => {
  let component: ErrorHandlerComponent;
  let fixture: ComponentFixture<ErrorHandlerComponent>;
  let activatedRoute: ActivatedRouteStub;
  let router: Router;

  beforeEach(() => {
    fixture = TestBed.createComponent(ErrorHandlerComponent);
    component = fixture.componentInstance;
    activatedRoute = new ActivatedRouteStub({ errorCode: '123', exceptionId:'23FC49D7-EA16-4698-BACE-0214B22B3CDF', message:'error message' });
    router = TestBed.get(Router);
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

});
