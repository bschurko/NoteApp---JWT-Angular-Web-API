using Microsoft.EntityFrameworkCore;
using NoteApp.Server.Data;
using NoteApp.Server.Models;


namespace NoteApp.Server.Services
{
    public class NoteService : IRepository
    {
        private AppDbContext _context;
        public NoteService(AppDbContextFactory factory)
        {
            _context = factory.CreateDbContext(null);
        }
         
        public async Task<Note> AddAsync(Note entity)
        {
            if (entity == null) return null;
            var res = await _context.Notes.AddAsync(entity).ConfigureAwait(false);
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return res.Entity;
        }

        public async Task<Note> UpdateAsync(Note entity, int id)
        {
            var note = await _context.Notes.FindAsync(id).ConfigureAwait(false);
            if (note != null)
            {
                note.Title = entity.Title;
                note.Content = entity.Content;
                note.Author = entity.Author;
                note.DateUpdated = DateTime.Now;
                note.ImagePath = entity.ImagePath;
                note.IsPinned = entity.IsPinned;
                note.IsArchived = entity.IsArchived;
                note.Tags = entity.Tags;
                await _context.SaveChangesAsync().ConfigureAwait(false);
         
                return note;
            }

            return null;
        }

        public async Task DeleteAsync(int id)
        {
            var note = await _context.Notes.FindAsync(id).ConfigureAwait(false);
            if (note != null) _context.Notes.Remove(note);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            try
            {
                return await _context.Notes.ToListAsync().ConfigureAwait(false);
            }
            catch (Exception error)
            {
                Console.WriteLine("Error getting all note entities from Db.", error);
                throw error;
            }
        }

        public async Task<Note> GetByIdAsync(int id)
        {
            var note = _context.Notes.FirstOrDefault(cc => cc.Id == id);
            return note;
        }
 
    }

    public interface IRepository
    {
        Task<Models.Note> AddAsync(Models.Note entity);

        Task<Models.Note> UpdateAsync(Models.Note entity, int id);

        Task DeleteAsync(int id);

        Task<IEnumerable<Models.Note>> GetAllAsync();

        Task<Models.Note> GetByIdAsync(int id);

    }
}
