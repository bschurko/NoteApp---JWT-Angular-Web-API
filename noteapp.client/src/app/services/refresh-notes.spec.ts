import { TestBed } from '@angular/core/testing';

import { RefreshNotes } from './refresh-notes';

describe('RefreshNotes', () => {
  let service: RefreshNotes;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RefreshNotes);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
