import { Component } from '@angular/core';

@Component({
  selector: 'app-filter-notes',
  standalone: false,
  templateUrl: './filter-notes.html',
  styleUrl: './filter-notes.css',
})
export class FilterNotes {
  selectedOption: string = 'all';

  filterOptions = [
    { label: 'All Tasks', value: 'all' },
    { label: 'Pending', value: 'pending' },
    { label: 'Archived', value: 'archived' }
  ];

}
