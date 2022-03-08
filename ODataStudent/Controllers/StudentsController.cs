using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataStudent.Data;
using ODataStudent.Models;

namespace ODataStudent.Controllers
{
    public class StudentsController: ODataController
    {
        private readonly StudentDataContext _db;
        private readonly ILogger<StudentsController> _logger;
        public StudentsController(StudentDataContext dbContext, ILogger<StudentsController> logger)
        {
            _logger = logger;
            _db = dbContext;
        }

        
        [EnableQuery(PageSize = 2)]
        public IQueryable<Student> Get()
        {
            return _db.Students;
        }

        [EnableQuery]
        public SingleResult<Student> Get([FromODataUri] int key)
        {
            var result = _db.Students.Where(c => c.Id == key);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
            return Created(student);
        }

        [EnableQuery]
        public async Task<IActionResult> Patch([FromODataUri] int key, Delta<Student> note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingNote = await _db.Students.FindAsync(key);
            if (existingNote == null)
            {
                return NotFound();
            }

            note.Patch(existingNote);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Updated(existingNote);
        }

        [EnableQuery]
        public async Task<IActionResult> Delete([FromODataUri] int key)
        {
            Student existingNote = await _db.Students.FindAsync(key);
            if (existingNote == null)
            {
                return NotFound();
            }

            _db.Students.Remove(existingNote);
            await _db.SaveChangesAsync();
            return StatusCode(StatusCodes.Status204NoContent);
        }

        private bool NoteExists(int key)
        {
            return _db.Students.Any(p => p.Id == key);
        }

    }
}
