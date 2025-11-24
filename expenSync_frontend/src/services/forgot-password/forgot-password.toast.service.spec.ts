import { TestBed } from '@angular/core/testing';

import { ForgotPasswordToastService } from './forgot-password.toast.service';

describe('ForgotPasswordToastService', () => {
  let service: ForgotPasswordToastService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ForgotPasswordToastService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
