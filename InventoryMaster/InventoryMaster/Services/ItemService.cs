using InventoryMaster.Dtos;
using InventoryMaster.Entities;
using InventoryMaster.Interfaces;
using InventoryMaster.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace InventoryMaster.Services
{
    public class ItemService : IItemService //Сервис для добавления предмета в базу данных
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
                _logger.LogInformation($"Предмет успешно добавлен в базу данных!");
                return newItem;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка при добавлении предмета в базу данных: {ex.Message}");
                return newItem;
            }
        }

        public async Task<Item?> UpdateItemAsync(Guid id, [FromBody] ItemDto itemUpdateDto)
        {
            try
            {
                if (itemUpdateDto == null || itemUpdateDto.Quantity <= 0 || itemUpdateDto.Price <= 0 || string.IsNullOrWhiteSpace(itemUpdateDto.Name.Trim()))
                    throw new Exception();


                Item? existingItem = await _context.Items.FirstOrDefaultAsync(item => item.Id == id)
                ?? throw new NullReferenceException();
                   

                TypeOfItems? type = await _context.TypeOfItems.FirstOrDefaultAsync(t => t.Name.Trim().ToLower() == itemUpdateDto.TypeName.Trim().ToLower())
                ?? throw new NullReferenceException();


                existingItem.Name = itemUpdateDto.Name.Trim();
                existingItem.Quantity = itemUpdateDto.Quantity;
                existingItem.TypeOfItemsId = type.TypeId;
                existingItem.Type = type;
                existingItem.Price = itemUpdateDto.Price;

                await _context.SaveChangesAsync();
                return existingItem;
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, " NullReferenceException occurred while updating item with ID: {ItemId}", id);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Unexpected error occurred while updating item with ID: {ItemId}", id);
                return null;
            }
        }

        public async Task<List<Item>?> GetItemsAsync()
        {
            try
            {
                List<Item> items = await _context.Items.Include(i => i.Type).ToListAsync();

                if (items == null || items.Count == 0)
                    return new List<Item>();

                else
                    return items;

            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при получении списка предметов:", ex);
                return null;
            }
        }



        public async Task<Item?> DeleteItemAsync(Guid Id, int Quantity)
        {
            try
            {
                if (Quantity <= 0)
                    throw new ArgumentOutOfRangeException(nameof(Quantity), "Количество должно быть положительным числом.");

                Item? itemToDelete = await _context.Items.FirstOrDefaultAsync(i => i.Id == Id);

                if (itemToDelete == null)
                    return null;

                if (Quantity >= itemToDelete.Quantity)
                {
                    _context.Items.Remove(itemToDelete);
                }
                else
                {
                    itemToDelete.Quantity -= Quantity;
                    _context.Items.Update(itemToDelete);
                }

                await _context.SaveChangesAsync();
                return itemToDelete;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogError(ex, "Неправильно указано значение Quantity");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении предмета");
                return null;
            }
        }



        public async Task<Item?> PostItemAsync(ItemDto itemDto)  // Добавление нового предмета в базу данных
        {
            try
            {
                if (string.IsNullOrWhiteSpace(itemDto.Name) || itemDto.Quantity <= 0 || string.IsNullOrEmpty(itemDto.TypeName) || itemDto.Price <= 0 || itemDto.Name == "string")
                    return null;

                itemDto.Name = itemDto.Name.Trim();
                itemDto.TypeName = itemDto.TypeName.Trim();
                TypeOfItems? existingType = await _context.TypeOfItems.FirstOrDefaultAsync(t => t.Name == itemDto.TypeName);

                if (existingType == null)
                   return null;

                Item newItem = new(itemDto.Name, itemDto.Quantity, existingType, itemDto.Price);

                Item result = await TryAddItemToDBAsync(newItem);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при добавлении предмета" ,ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteAllItemsAsync() // Удаления всех предметов из базы данных
        {
            try
            {
                _context.Items.RemoveRange(_context.Items);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при удалении всех предметов:" ,ex.Message);
                return false;
            }
        }

        public async Task<List<Item>> SerchAsync<T> (T Value, ItemFields SearchType)
        {
            List<Item> ItemList = new();
            switch (SearchType)
            {
                case ItemFields.Id:
                    if (Value is Guid id)
                        ItemList = await _context.Items.Include(item => item.Type).Where(i => i.Id == id).ToListAsync();
                        return ItemList;
                case ItemFields.Name:
                    {
                        if(Value is string name)
                        ItemList = await _context.Items.Include(item => item.Type).Where(i => i.Name.ToLower() == name.ToLower().Trim()).ToListAsync();
                        return ItemList;
                    }
                case ItemFields.Quantity:
                    {
                        if (Value is int quantity)
                            ItemList = await _context.Items.Include(item => item.Type).Where(i => i.Quantity == quantity).ToListAsync();
                        return ItemList;
                    }
                case ItemFields.Type:
                    {
                        if (Value is string type)
                            ItemList = await _context.Items.Include(item => item.Type).Where(item => item.Type.Name.ToLower() == type.ToLower().Trim()).ToListAsync();
                            return ItemList;
                    }
                case ItemFields.Price:
                    {
                        if (Value is double price)
                            ItemList = await _context.Items.Include(item => item.Type).Where(i => i.Price == price).ToListAsync();
                        return ItemList;
                    }
                default: return ItemList;
            }
        }


        public async Task<List<Item>> SortAsync (ItemFields FieldForSort, bool Descending)
        {
            List<Item> SortedList = new();

            switch (FieldForSort)
            {
                case ItemFields.Id:
                {
                    if (!Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderBy(item => item.Id).ToListAsync();

                    else if (Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Id).ToListAsync();

                        return SortedList;
                }
                case ItemFields.Name:
                    {
                        if (!Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderBy(item => item.Name).ToListAsync();

                        else if (Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Name).ToListAsync();

                        return SortedList;
                    }
                case ItemFields.Quantity:
                    {
                        if (!Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderBy(item => item.Quantity).ToListAsync();

                        else if (Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Quantity).ToListAsync();

                        return SortedList;
                    }
                case ItemFields.Type:
                    {
                        if (!Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderBy(item => item.Type).ToListAsync();

                        else if (Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Type).ToListAsync();

                        return SortedList;
                    }
                case ItemFields.Price:
                    {
                        if (!Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderBy(item => item.Price).ToListAsync();
                        
                        else if (Descending)
                        SortedList = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Price).ToListAsync();

                        return SortedList;
                    }
            }
            return SortedList;
        }
    }
}



