using InventoryMaster.Dtos;
using InventoryMaster.Model;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMaster.Interfaces
{
    public interface IItemService // Интерфейс для сервиса TryAddItemToDB
    {
        Task<Item> TryAddItemToDBAsync(Item newItem);
        Task<Item?> UpdateItemAsync(Guid id, [FromBody] ItemDto itemUpdateDto);
        Task<List<Item>?> GetItemsAsync();
        Task<Item?> DeleteItemAsync(Guid Id, int Quantity);
    }
}
