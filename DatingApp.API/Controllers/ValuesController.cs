using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // Inject the Data Context class into the controller to get data from the  dn:
        private readonly DataContext _context;
        public ValuesController(DataContext context)
        {
            _context = context;

        }
        // GET api/values
        // use IActionResult type to return HTTP responses in controller
        // make methods async to avoid blocking the thread on db queries - a Task represents an asynchronous op that can return a value
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            // toList executes query to db and returns records from the table Values in a list.
            var values = await _context.Values.ToListAsync();

            return Ok(values);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(int id)
        {
            // FirstOrDefault will return null if no record found instead of throw an exception (as with First)- 
            // this is preferred since unecessarily throwing exceptions is expensive
            var value = await _context.Values.FirstOrDefaultAsync(x => x.Id == id);

            // A null value returned from Ok will return a 204 No Content response - successful but empty
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
