import { TestBed } from '@angular/core/testing';

import { CageService } from './cage.service';

describe('CageService', () => {
  let service: CageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
