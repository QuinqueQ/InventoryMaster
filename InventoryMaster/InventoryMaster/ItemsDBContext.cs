using InventoryMaster.Entities;
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
        public DbSet<TypeOfItems> TypeOfItems { get; set; } //  DbSet, представляющий таблицу "TypeOfItems" в базе данных

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TypeOfItems>().HasKey(t => t.TypeId); // Указываем первичный ключ для TypeOfItems
        }
    }
}
