import { Component, Input, OnInit } from '@angular/core';
import { Note } from '../../Models/Note';
import { BroadcastService } from '../../services/broadcast-service'
import { NoteService } from '../../services/note-service';
import { RefreshNotes } from '../../services/refresh-notes';

@Component({
  selector: 'app-edit-notes',
  standalone: false,
  templateUrl: './edit-notes.html',
  styleUrl: './edit-notes.css',
})
export class EditNotes implements OnInit {

  note: Note = new Note();
  isNewNote: boolean = false;
  isEditing: boolean = false;

  constructor(
    private broadcastService: BroadcastService,
    private noteService: NoteService,
    private refreshNotesBroadcast: RefreshNotes) { }

  ngOnInit() {
    this.broadcastService.sourceMessage.subscribe(msg => this.note = msg);
    this.note.id = "0";
  }

  onEditNote() {
    this.isEditing = true;
    this.isNewNote = false;
  }

  onCreateNote() {
    this.isNewNote = true;
    this.isEditing = false;
    this.note = new Note();
    this.note.id = "0";
    this.note.dateUpdated = new Date();
  }

  onSaveNote() {

    if (this.isNewNote) {
        this.noteService.addNote(this.note).subscribe(cc => {
          this.note = cc;
          this.noteService.clearCache();
          this.refreshNotesBroadcast.RefreshNotesMessage(true)

        });
    } else if (this.isEditing) {
      this.noteService.updateNote(parseInt(this.note.id), this.note).subscribe(cc => {
        this.note = cc;
        this.noteService.clearCache();
        this.refreshNotesBroadcast.RefreshNotesMessage(true)
      });
    }

    this.isEditing = false;
    this.isNewNote = false;
  }
}
