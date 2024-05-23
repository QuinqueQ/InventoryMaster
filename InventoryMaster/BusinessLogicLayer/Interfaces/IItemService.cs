using DomainLayer.Dtos;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BusinessLogicLayer.Interfaces
{
    public interface IItemService // Интерфейс для сервиса предметов
    {
        Task<Item> TryAddItemToDBAsync(Item newItem);
        Task<Item?> UpdateItemAsync(Guid id, [FromBody] ItemDto itemUpdateDto);
        Task<List<Item>?> GetItemsAsync();
        Task<Item?> DeleteItemAsync(Guid Id, int Quantity);
        Task<Item?> PostItemAsync(ItemDto itemDto);
        Task<bool> DeleteAllItemsAsync();
        Task<List<Item>> SerchAsync<T>(T Value, ItemFields SearchType);
        Task<List<Item>> SortAsync(ItemFields FieldForSort, bool Descending);
    }
}
