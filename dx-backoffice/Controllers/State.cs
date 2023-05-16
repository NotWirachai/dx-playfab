using dx_backoffice.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using dx_backoffice.Data;

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

        [HttpPost]
        public IActionResult CreateState([FromBody]StateModel state)
        {
            _db.State.Add(state);
            _db.SaveChanges();
            return Ok();
        }

    }
}
