using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using NoteApp.Server.Data;
using NoteApp.Server.Models;
using NoteApp.Server.Services;

namespace NoteApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private IRepository _noteService;
        private IConfiguration _config;
        
        public NotesController(IRepository NoteService, IConfiguration config)
        {
            _noteService = NoteService;
            _config = config;
        }

        [Authorize]
        [Route("GetNotes")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {

            try
            {
                var notes = await _noteService.GetAllAsync();
                return notes.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.InnerException?.Message);
                Console.WriteLine(ex.InnerException?.InnerException?.Message);
                throw ex;
            }
        }

        [Authorize]
        [Route("GetNote/{id?}")]
        [HttpGet()]
        public async Task<ActionResult<Note>> Get(int? id = null)
        {
            if (id != null)
            {
                return await _noteService.GetByIdAsync(id.Value);
            }

            return NotFound();
        }

        [Authorize]
        [Route("AddNote")]
        [HttpPost()]
        public async Task<ActionResult<Note>> Post(Note note)
        {
            var res = await _noteService.AddAsync(note);
            return res;
        }

        [Authorize]
        [Route("DeleteNote/{id}")]
        [HttpDelete()]
        public async Task<ActionResult> Delete([FromRoute] int Id)
        {
            var note = await _noteService.GetByIdAsync(Id);
            await _noteService.DeleteAsync(note.Id);
            return Ok();
        }

        [Authorize]
        [Route("UpdateNote/{id}")]
        [HttpPut()]
        public async Task<ActionResult<Note>> Update([FromRoute] int id, [FromBody] Note note)
        {
            if (note == null) return BadRequest();
            var updatedNote = await _noteService.UpdateAsync(note, id);
            return updatedNote;
        }
    }
}
