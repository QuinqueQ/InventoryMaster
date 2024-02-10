﻿using InventoryMaster.Interfaces;
using InventoryMaster.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace InventoryMaster.Services
{
    public class ItemService : IItemService //сервис для создания предметов в бд
    {
        private readonly ItemsDBContext _context;
        private readonly ILogger<ItemService> _logger;
        public ItemService(ItemsDBContext context, ILogger<ItemService> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<Item> TryAddItemToDBAsync(Item newItem)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при добавлении предмета в базу данных: {ex.Message}");
               return new Item();
            }
        }
    }
}
