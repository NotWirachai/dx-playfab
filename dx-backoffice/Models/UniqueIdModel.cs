namespace dx_backoffice.Models
{
    public class UniqueIdModel
    {
        public string uniqueId { get; set; }
        public void GenerateUniqueId()
        {
            // Generate a new UUID
            uniqueId = Guid.NewGuid().ToString();
        }

    }
}
