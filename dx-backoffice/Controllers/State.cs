using dx_backoffice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using dx_backoffice.Data;
using Microsoft.EntityFrameworkCore;

namespace dx_backoffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class State : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public State(ApplicationDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetState()
        {
            List<StateModel> state = await _db.State.ToListAsync();
            return Ok(state);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStateById(int id)
        {
            // Retrieve the state record by ID
            var state = await _db.State.FindAsync(id);
            if (state == null)
            {
                return NotFound("State not found.");
            }

            return Ok(state);
        }

        [HttpPost]
        public async Task<IActionResult> CreateState([FromBody] StateModel state)
        {
            // Check if the provided ID already exists in the database
            var existingState = await _db.State.FindAsync(state.Id);
            if (existingState != null)
            {
                return Conflict("State with the same ID already exists.");
            }

            _db.State.Add(state);
            await _db.SaveChangesAsync();

            return Ok("state created successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateState(int id, [FromBody] StateModel updatedState)
        {
            // Check if the provided ID exists in the database
            var existingState = await _db.State.FindAsync(id);
            if (existingState == null)
            {
                return NotFound("State not found.");
            }

            // Update the existing state with the new values
            existingState.State = updatedState.State;
            // Update other properties as needed...

            await _db.SaveChangesAsync();

            return Ok("State updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteState(int id)
        {
            // Find the state with the specified ID
            var state = await _db.State.FindAsync(id);
            if (state == null)
            {
                return NotFound("State not found.");
            }

            // Remove the state from the database
            _db.State.Remove(state);
            await _db.SaveChangesAsync();

            return Ok("State deleted successfully.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllStates()
        {
            // Retrieve all state records
            var states = await _db.State.ToListAsync();

            // Remove all states from the database
            _db.State.RemoveRange(states);
            await _db.SaveChangesAsync();

            return Ok("All states deleted successfully.");
        }


    }
}
