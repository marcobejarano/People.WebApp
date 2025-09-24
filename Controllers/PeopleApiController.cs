using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using People.WebApp.Data;
using People.WebApp.Helpers;
using People.WebApp.Models;
using System.Net;

namespace People.WebApp.Controllers
{
    [Route("api/people")]
    [ApiController]
    public class PeopleApiController : ControllerBase
    {
        private readonly PeopleDbContext context;

        public PeopleApiController(PeopleDbContext context)
        {
            this.context = context;
        }

        // GET: api/PeopleApi
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IEnumerable<Person>>> GetPeople()
        {
            if (context.People == null)
            {
                return NotFound();
            }
            return await context.People.ToListAsync();
        }

        // GET: api/PeopleApi/5
        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Person>> GetPerson(string id)
        {
            if (context.People == null)
            {
                return NotFound();
            }

            var person = await context.People.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // PUT: api/PeopleApi/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PutPerson(string id, Person person)
        {            
            if (id != person.Id)
            {
                return BadRequest();
            }

            var personToUpdate = await context.People.FindAsync(id);
            if (personToUpdate == null)
            {
                return NotFound();
            }

            personToUpdate.Update(person);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!PeopleDbContextHelper.PersonExists(context, person.Id))
            {
                return NotFound();
            }
            
            return NoContent();
        }

        // POST: api/PeopleApi
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<ActionResult<Person>> PostPerson(Person person)
        {
            if (context.People == null)
            {
                return Problem("Entity set 'PeopleDbContext.People'  is null.");
            }

            if (string.IsNullOrEmpty(person.Id))
            {
                person.Id = Guid.NewGuid().ToString();
            }

            context.People.Add(person);

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PeopleDbContextHelper.PersonExists(context, person.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        // DELETE: api/PeopleApi/5
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeletePerson(string id)
        {
            if (context.People == null)
            {
                return NotFound();
            }
            var person = await context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            context.People.Remove(person);
            await context.SaveChangesAsync();

            return NoContent();
        }        
    }
}
