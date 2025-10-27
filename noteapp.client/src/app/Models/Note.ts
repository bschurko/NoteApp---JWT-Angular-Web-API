 

export interface INote {
  id: string;
  title: string;
  content: string;
  dateCreated?: Date | null;
  dateUpdated?: Date | null;
  imagePath: string | null;
  tags?: string;
  isPinned?: boolean;
  isArchived?: boolean;
}

export class Note implements INote{
  id: string;
  title: string;
  content: string;
  dateCreated?: Date | null;
  dateUpdated?: Date | null;
  imagePath: string | null = null;
  tags: string;
  isPinned: boolean;
  isArchived: boolean;

  /**
   * Create a new Note instance.
   * @param init Partial initialization object or INote
   */
  constructor(init?: Partial<INote> | Partial<Note>) {
    this.id = (init && ('id' in init ? String((init as any).id) : undefined)) ?? generateId();
    this.title = (init && (init as any).title) ?? '';
    this.content = (init && (init as any).content) ?? '';
    this.dateCreated = parseDate((init && (init as any).dateCreated) ?? new Date().toISOString());
    this.dateUpdated = (init && (init as any).dateUpdated) ? parseDate((init as any).dateUpdated) : undefined;
    this.tags = (init && (init as any).tags) ?? '';
    this.isPinned = (init && (init as any).isPinned) ?? false;
    this.isArchived = (init && (init as any).isArchived) ?? false;
  }

  /**
   * Create a Note from a plain JS object (e.g., parsed JSON).
   */
  static fromJSON(obj: INote | any): Note {
    return new Note({
      id: obj?.id,
      title: obj?.title,
      content: obj?.content,
      dateCreated: obj?.dateCreated,
      dateUpdated: obj?.dateUpdated,
      tags: obj?.tags,
      isPinned: obj?.isPinned,
      isArchived: obj?.isArchived,
    });
  }

  /**
   * Convert to a plain object suitable for JSON serialization.
   */
  toJSON(): INote {
    return {
      id: this.id,
      title: this.title,
      content: this.content,
      dateCreated: this.dateCreated,
      dateUpdated: this.dateUpdated,
      imagePath: this.imagePath,
      tags: this.tags,
      isPinned: this.isPinned,
      isArchived: this.isArchived,
    };
  }

  /**
   * Update fields from another note or partial data.
   */
  update(values: Partial<INote> | Partial<Note>): void {
    if ((values as any).title !== undefined) this.title = (values as any).title;
    if ((values as any).content !== undefined) this.content = (values as any).content;
    if ((values as any).tags !== undefined) this.tags = (values as any).tags;
    if ((values as any).isPinned !== undefined) this.isPinned = (values as any).isPinned;
    if ((values as any).isArchived !== undefined) this.isArchived = (values as any).isArchived;
    if ((values as any).dateUpdated !== undefined) {
      this.dateUpdated = parseDate((values as any).dateUpdated);
    } else {
      this.dateUpdated = new Date();
    }
  }

  /**
   * Create a shallow clone of this note.
   */
  clone(): Note {
    return new Note(this.toJSON());
  }

  /**
   * Simple validation: ensures title or content exists.
   */
  isValid(): boolean {
    return Boolean(this.title && this.title.trim().length > 0) || Boolean(this.content && this.content.trim().length > 0);
  }
}

/* Helper functions */

function parseDate(input: string | Date | undefined): Date {
  if (!input) return new Date();
  if (input instanceof Date) return input;
  const d = new Date(input);
  if (isNaN(d.getTime())) return new Date();
  return d;
}

function generateId(): string {
  // Lightweight unique id generator (not cryptographically secure)
  return 'note_' + Date.now().toString(36) + '_' + Math.random().toString(36).slice(2, 9);
}
