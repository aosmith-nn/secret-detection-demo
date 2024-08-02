using Microsoft.EntityFrameworkCore;

namespace hello_world_api.Models
{
    public class HelloDbContext : DbContext
    {
        public HelloDbContext() { }

        public void Update<T>(string field, string value)
        {
            
        }
    }
}
