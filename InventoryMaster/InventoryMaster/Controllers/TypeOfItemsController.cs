using InventoryMaster.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TypeOfItemsController : ControllerBase
    {
        private readonly ItemsDBContext _context;

        public TypeOfItemsController(ItemsDBContext context)
        {
            _context = context;
        }

        [HttpGet(Name = "TypeGet")]
        public async Task<IActionResult> Get()  // получаем все типы предметов
        {
            if (!_context.TypeOfItems.Any())
                return Ok("Ваша база пуста!");

            return Ok(await _context.TypeOfItems.ToListAsync());
        }


        [HttpPost(Name = "TypePost")]
        public async Task<IActionResult> PostType(string? TypeName) // создание нового типа предмета
        {
            if (string.IsNullOrWhiteSpace(TypeName))
                return BadRequest("Заполните Название!");

            TypeName = TypeName.Trim();
            if (_context.TypeOfItems.Any(type => type.Name.ToLower() == TypeName.ToLower()))
            {
                return BadRequest("Такой тип уже существует!");
            }
            TypeOfItems newType = new(TypeName);
            _context.TypeOfItems.Add(newType);
            await _context.SaveChangesAsync();
            return Ok(newType);
        }

        [HttpDelete(Name = "TypeDelete")]
        public async Task<IActionResult> DeleteTypeById(int typeId) // удаление предмета по айди
        {
            try
            {
                TypeOfItems? typeToDelete = await _context.TypeOfItems.FindAsync(typeId);

                if (typeToDelete == null)
                    return NotFound("Тип предмета не найден!");

                _context.TypeOfItems.Remove(typeToDelete);
                await _context.SaveChangesAsync();

                return Ok("Тип предмета успешно удален!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении типа предмета: {ex.Message}");
            }
        }
        [HttpPut(Name ="TypeUpdate")]
        public async Task<IActionResult> UpdateType(int id, string? UpdTypeName)
        {
            try
            {
                TypeOfItems? existingItem = await _context.TypeOfItems.FindAsync(id);

                if (existingItem == null)
                    return BadRequest("Тип с таким идентификатором не найден!");

                if (string.IsNullOrWhiteSpace(UpdTypeName))
                    return BadRequest("Название типа не может быть пустым или содержать только пробелы!");

                UpdTypeName = UpdTypeName.Trim();
                existingItem.Name = UpdTypeName;

                await _context.SaveChangesAsync();

                return Ok(existingItem);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении типа: {ex.Message}");
            }
        }
    }
}
