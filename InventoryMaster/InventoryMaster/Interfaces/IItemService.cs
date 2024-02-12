using InventoryMaster.Model;

namespace InventoryMaster.Interfaces
{
    public interface IItemService // Интерфейс для сервиса TryAddItemToDB
    {
        Task<Item> TryAddItemToDBAsync(Item newItem);
    }
}
