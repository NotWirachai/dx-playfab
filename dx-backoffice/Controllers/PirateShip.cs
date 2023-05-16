using dx_backoffice.Data;
using dx_backoffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dx_backoffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PirateShip : ControllerBase
    {
        private readonly ApplicationDBContext _db;

        public PirateShip(ApplicationDBContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetPirateShips()
        {
            List<PirateShipModel> pirateShips = await _db.PirateShips.ToListAsync();
            return Ok(pirateShips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPirateShipsById(int id)
        {
            // Retrieve the state record by ID
            var PirateShips = await _db.PirateShips.FindAsync(id);
            if (PirateShips == null)
            {
                return NotFound("PirateShips not found.");
            }

            return Ok(PirateShips);
        }

        [HttpPost]
        public IActionResult CreatePirate([FromBody] PirateShipModel pirate)
        {
            _db.PirateShips.Add(pirate);
            _db.SaveChanges();
            return Ok("pirate created successfully. ID: " + pirate.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePirateShip(int id, [FromBody] PirateShipModel updatedPirateShip)
        {
            var pirateShip = await _db.PirateShips.FindAsync(id);

            if (pirateShip == null)
            {
                return NotFound();
            }

            pirateShip.Level = updatedPirateShip.Level;
            pirateShip.TotalPower = updatedPirateShip.TotalPower;
            pirateShip.BuildingUnlocked = updatedPirateShip.BuildingUnlocked;
            pirateShip.ConstructionTime = updatedPirateShip.ConstructionTime;
            pirateShip.UseGemToFinish = updatedPirateShip.UseGemToFinish;

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePirateShips(int id)
        {
            // Find the state with the specified ID
            var PirateShips = await _db.PirateShips.FindAsync(id);
            if (PirateShips == null)
            {
                return NotFound("State not found.");
            }

            // Remove the state from the database
            _db.PirateShips.Remove(PirateShips);
            await _db.SaveChangesAsync();

            return Ok("State deleted successfully.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllPirateShips()
        {
            // Retrieve all state records
            var PirateShips = await _db.PirateShips.ToListAsync();

            // Remove all states from the database
            _db.PirateShips.RemoveRange(PirateShips);
            await _db.SaveChangesAsync();

            return Ok("All states deleted successfully.");
        }

    }
}
