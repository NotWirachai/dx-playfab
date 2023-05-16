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

        [HttpPost]
        public IActionResult CreatePirate([FromBody] PirateShipModel pirate)
        {
            _db.PirateShips.Add(pirate);
            _db.SaveChanges();
            return Ok("pirate created successfully. ID: " + pirate.Id);
        }
    }
}
