using InventoryMaster.Model;

namespace InventoryMaster.Interfaces
{
    public interface IItemService
    {
        Task<Item> TryAddItemToDBAsync(Item newItem);
    }
}
