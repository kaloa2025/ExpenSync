import { TestBed } from '@angular/core/testing';

import { DashboardRecentTransactionsService } from './dashboard.recent-transactions.service';

describe('DashboardRecentTransactionsService', () => {
  let service: DashboardRecentTransactionsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DashboardRecentTransactionsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
