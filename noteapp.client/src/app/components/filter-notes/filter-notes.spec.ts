import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FilterNotes } from './filter-notes';

describe('FilterNotes', () => {
  let component: FilterNotes;
  let fixture: ComponentFixture<FilterNotes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FilterNotes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FilterNotes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
