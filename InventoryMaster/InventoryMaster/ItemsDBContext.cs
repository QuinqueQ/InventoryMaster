using Microsoft.EntityFrameworkCore;

namespace InventoryMaster
{
    public class ItemsDBContext : DbContext
    {
        public ItemsDBContext(DbContextOptions<ItemsDBContext> options) : base(options)
        {

        }
    }
  
}
