using InventoryMaster.Entities;
using InventoryMaster.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaster.Services
{
    public class TypeOfItemService : ITypeOfItemService
    {
        private readonly ItemsDBContext _context;
        private readonly ILogger<TypeOfItemService> _logger;
        public TypeOfItemService(ILogger<TypeOfItemService> logger, ItemsDBContext context)
        {
            _logger = logger;
            _context = context;
        }
        public async Task<List<TypeOfItems>> GetTypeOfItemsAsync()
        {
            return await _context.TypeOfItems.ToListAsync();
        }

        public async Task<TypeOfItems?> PostTypeAsync(string? TypeName) // Создание нового типа предмета
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TypeName))
                   throw new ArgumentNullException(nameof(TypeName));

                    TypeName = TypeName.Trim();

                if (_context.TypeOfItems.Any(type => type.Name.ToLower() == TypeName.ToLower()))
                    return null;

                TypeOfItems newType = new(TypeName);
                _context.TypeOfItems.Add(newType);
                await _context.SaveChangesAsync();
                return newType;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "Поле заполненно не корректно!");
                return null;
            }
            catch (Exception )
            {
                _logger.LogInformation("Данный тип уже существует!");
                return null;
            }
            
        }
        public async Task<TypeOfItems?> DeleteTypeByIdAsync(int typeId) // Удаление предмета по айди
        {
            try
            {
                TypeOfItems? typeToDelete = await _context.TypeOfItems.FindAsync(typeId) ?? throw new ArgumentNullException(nameof(typeId));

                _context.TypeOfItems.Remove(typeToDelete);
                await _context.SaveChangesAsync();

                return typeToDelete;
            }
            catch(ArgumentNullException ex)
            {
                _logger.LogError(ex, "Поле заполненно не корректно!");
                return null;
            }
            catch (Exception ex)
            {
               _logger.LogError($"Ошибка при удалении типа предмета:", ex.Message);
                return null;
            }
        }

        public async Task<TypeOfItems?> UpdateTypeAsync(int id, string? UpdTypeName) //Изменение типа предмета по айди
        {
            try
            {
                TypeOfItems? existingItem = await _context.TypeOfItems.FindAsync(id) ?? throw new ArgumentNullException(nameof(id));

                if (string.IsNullOrWhiteSpace(UpdTypeName))
                    throw new ArgumentNullException(nameof(UpdTypeName));

                UpdTypeName = UpdTypeName.Trim();
                existingItem.Name = UpdTypeName;

                await _context.SaveChangesAsync();

                return existingItem;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation(ex, "Неправильное заполнение!");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка при обновлении типа:",ex.Message);
                return null;
            }
        }
    }
}
