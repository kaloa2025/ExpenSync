import { TestBed } from '@angular/core/testing';

import { DashboardProfileService } from './dashboard.profile.service';

describe('DashboardProfileService', () => {
  let service: DashboardProfileService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DashboardProfileService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
