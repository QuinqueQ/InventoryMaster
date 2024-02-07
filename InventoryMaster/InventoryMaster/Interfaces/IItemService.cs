using InventoryMaster.Model;

namespace InventoryMaster.Services
{
    public interface IItemService
    {
        Task<Item> TryAddItemToDBAsync(Item newItem);
    }
}
