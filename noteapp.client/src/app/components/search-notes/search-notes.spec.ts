import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SearchNotes } from './search-notes';

describe('SearchNotes', () => {
  let component: SearchNotes;
  let fixture: ComponentFixture<SearchNotes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SearchNotes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SearchNotes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
