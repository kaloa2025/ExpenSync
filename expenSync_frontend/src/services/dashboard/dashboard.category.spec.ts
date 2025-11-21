import { TestBed } from '@angular/core/testing';

import { DashboardCategory } from './dashboard.category';

describe('DashboardCategory', () => {
  let service: DashboardCategory;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DashboardCategory);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
