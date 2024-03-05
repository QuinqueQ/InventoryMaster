using InventoryMaster.Entities;

namespace InventoryMaster.Interfaces
{
    public interface ITypeOfItemService
    {
        Task<List<TypeOfItems>> GetTypeOfItemsAsync();
        Task<TypeOfItems?> PostTypeAsync(string? TypeName);
        Task<TypeOfItems?> DeleteTypeByIdAsync(int typeId);
        Task<TypeOfItems?> UpdateTypeAsync(int id, string? UpdTypeName);
    }
}
