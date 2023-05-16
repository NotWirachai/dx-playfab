using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace dx_backoffice.Models
{
    public class StateModel
    {
        [Key]
        public int Id { get; set; }
        public string State { get; set; }

        public string GetRandomState()
        {
            string[] states = { "State1", "State2", "State3", "State4", "State5", "State6" };

            Random random = new Random();
            int index = random.Next(0, states.Length);

            return states[index];
        }
    }
}
