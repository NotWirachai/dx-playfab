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

        [HttpPost]
        public IActionResult CreateState([FromBody] StateModel state)
        {
            _db.State.Add(state);
            _db.SaveChanges();
            return Ok();
        }

    }
}
