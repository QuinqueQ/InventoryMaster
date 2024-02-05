using InventoryMaster.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaster
{
    public class ItemsDBContext : DbContext
    {
        public ItemsDBContext(DbContextOptions<ItemsDBContext> options) : base(options)
        {

        }
        public DbSet<Item> Items { get; set; } // DbSet, представляющий таблицу "Items" в базе данных
    }
  
}
