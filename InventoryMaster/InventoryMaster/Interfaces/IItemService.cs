using InventoryMaster.Model;

namespace InventoryMaster.Interfaces
{
    public interface IItemService // интерфейс для сервиса
    {
        Task<Item> TryAddItemToDBAsync(Item newItem);
    }
}
