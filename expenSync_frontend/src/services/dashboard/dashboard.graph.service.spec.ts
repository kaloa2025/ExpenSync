import { TestBed } from '@angular/core/testing';

import { DashboardGraphService } from './dashboard.graph.service';

describe('DashboardGraphService', () => {
  let service: DashboardGraphService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DashboardGraphService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
