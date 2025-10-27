import { Component, OnInit, signal, WritableSignal } from '@angular/core';
import { NoteService } from '../../services/note-service';
import { Note } from '../../Models/Note';
import { BroadcastService } from '../../services/broadcast-service';
import { RefreshNotes } from '../../services/refresh-notes';

@Component({
  selector: 'app-notes-list',
  standalone: false,
  templateUrl: './notes-list.html',
  styleUrl: './notes-list.css',
})
export class NotesList implements OnInit {

  notes: Note[] = [];
  selectedNote: Note | null = null;
  activeLinkId: string = '';

  constructor(private service: NoteService,
    private broadcastService: BroadcastService<Note>,
    private refreshNotesBroadcast: RefreshNotes) { }
   
  ngOnInit() {
 
    this.refreshNotes();
     

    this.refreshNotesBroadcast.refreshhNotes$.subscribe(cc => {
      if (cc === true) this.refreshNotes();
    });
  }

  refreshNotes() {
    this.service.getNotes().subscribe({
      next: (data) => this.notes = data,
      error: (err) => console.error('Error fetching notes', err)
    });
  }

  onNoteClicked(note: Note) {
    this.selectedNote = note;
    this.activeLinkId = note.id;
    this.broadcastService.broadcastMessage(note);
    console.log(note);
  }
}
