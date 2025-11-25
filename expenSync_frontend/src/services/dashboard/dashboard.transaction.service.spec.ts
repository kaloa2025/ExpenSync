import { TestBed } from '@angular/core/testing';

import { DashboardTransactionService } from './dashboard.transaction.service';

describe('DashboardTransactionService', () => {
  let service: DashboardTransactionService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DashboardTransactionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
