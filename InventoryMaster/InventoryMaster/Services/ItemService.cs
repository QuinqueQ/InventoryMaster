using InventoryMaster.Interfaces;
using InventoryMaster.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaster.Services
{
    public class ItemService : IItemService
    {
        private readonly ItemsDBContext _context;

        public ItemService(ItemsDBContext context)
        {
            _context = context;
        }

        public async Task<Item> TryAddItemToDBAsync(Item newItem)
        {
            // Проверяем, существует ли предмет с такими же характеристиками
            Item? existingItem = await _context.Items.FirstOrDefaultAsync(item =>
                item.Type == newItem.Type &&
                item.Name == newItem.Name &&
                item.Price == newItem.Price);

            if (existingItem != null)
            {
                // Обновляем количество существующего предмета
                existingItem.Quantity += newItem.Quantity;
                await _context.SaveChangesAsync();
                return existingItem;
            }

            // Если предмет не найден, добавляем его в базу данных
            _context.Items.Add(newItem);
            await _context.SaveChangesAsync();
            return newItem;
        }
    }
}
