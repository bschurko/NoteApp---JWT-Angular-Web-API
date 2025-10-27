import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditNotes } from './edit-notes';

describe('EditNotes', () => {
  let component: EditNotes;
  let fixture: ComponentFixture<EditNotes>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EditNotes]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EditNotes);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
