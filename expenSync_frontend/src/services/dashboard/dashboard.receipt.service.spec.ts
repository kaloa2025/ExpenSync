import { TestBed } from '@angular/core/testing';

import { DashboardReceiptService } from './dashboard.receipt.service';

describe('DashboardReceiptService', () => {
  let service: DashboardReceiptService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DashboardReceiptService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
