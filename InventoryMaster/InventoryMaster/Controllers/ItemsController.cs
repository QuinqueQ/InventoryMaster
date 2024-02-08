using InventoryMaster.Model;
using Microsoft.AspNetCore.Mvc;
using InventoryMaster.Interfaces;
using Microsoft.EntityFrameworkCore;
using InventoryMaster.Dtos;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ItemsDBContext _context; // контекст бд
        private readonly IItemService _itemService; //подключаю сервис для добавления предметов (он работает с колличеством предметов)

        public ItemsController(ItemsDBContext context, IItemService itemService)
        {
            _context = context;
            _itemService = itemService;
        }

        [HttpGet(Name = "GetItems")]
        public async Task<IActionResult> GetItems()
        {
            if (!_context.Items.Include(item => item.Type).Any())
                return Ok("Ваша база пуста!");
            try
            {
                var items = await _context.Items.Include(i => i.Type).ToListAsync(); 
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при получении списка предметов: {ex.Message}");
            }
        }

        [HttpPut("UpdateItem", Name = "UpdateItem")]
        [ProducesResponseType(typeof(Item), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemDto itemUpdateDto)
        {
            try
            {
                if (itemUpdateDto == null || itemUpdateDto.Quantity == 0 || itemUpdateDto.Price == 0)
                {
                    return BadRequest("Неверные данные для обновления предмета");
                }

                var existingItem = await _context.Items.FindAsync(id);

                if (existingItem == null)
                {
                    return NotFound($"Предмет с Id: {id} не найден");
                }

                existingItem.Name = itemUpdateDto.Name ?? existingItem.Name;
                existingItem.Quantity = itemUpdateDto.Quantity ?? 0;
                existingItem.Type = itemUpdateDto.Type;
                existingItem.Price = itemUpdateDto.Price ?? 0.0; 

                await _context.SaveChangesAsync();

                return Ok(existingItem);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при обновлении предмета: {ex.Message}");
            }
        }


        [HttpDelete(Name = "DeleteItem")] // удаление предмета по айди с выбором колличества удаляемых предметов
        public async Task<IActionResult> DeleteItem(Guid Id, int Quantity)
        {
            try
            {
                if (Quantity <= 0)
                    return BadRequest("Неверно указано количество!");

                Item? itemToDelete = await _context.Items.FirstOrDefaultAsync(i => i.Id == Id);

                if (itemToDelete == null)
                    return NotFound("Предмет не найден!");

                itemToDelete.Quantity -= Quantity;

                if (itemToDelete.Quantity <= 0)
                {
                    _context.Items.Remove(itemToDelete);
                    await _context.SaveChangesAsync();
                    return Ok("Предмет успешно удален!");
                }
                else
                {
                    _context.Items.Update(itemToDelete);
                    await _context.SaveChangesAsync();
                }

                return Ok(itemToDelete);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении предмета: {ex.Message}");
            }
        }

        [HttpGet("SortByNameAscending", Name = "SortByNameAscending")]
        public async Task<IActionResult> SortByNameAscending() // сортировка имени по алфавиту
        {
            try
            {
                var sortedItems = await _context.Items.Include(item => item.Type).OrderBy(item => item.Name).ToListAsync();

                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по имени (по возрастанию): {ex.Message}");
            }
        }

        [HttpGet("SortByNameDescending", Name = "SortByNameDescending")]
        public async Task<IActionResult> SortByNameDescending() // сортировка имени по алфавиту(наоборот)
        {
            try
            {
                var sortedItems = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Name).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по имени (по убыванию): {ex.Message}");
            }
        }

        [HttpGet("SortByType", Name = "SortByType")]
        public async Task<IActionResult> SortByType() //сортировка по типу предметов
        {
            try
            {
                var sortedItems = await _context.Items.Include(item => item.Type).OrderBy(item => item.Type).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по типу: {ex.Message}");
            }
        }

        [HttpGet("SortByPriceAscending", Name = "SortByPriceAscending")]
        public async Task<IActionResult> SortByPriceAscending() //сортировка по типу цене (уменьшение)
        {
            try
            {
                var sortedItems = await _context.Items.Include(item => item.Type).OrderBy(item => item.Price).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по цене (по возрастанию): {ex.Message}");
            }
        }

        [HttpGet("SortByPriceDescending", Name = "SortByPriceDescending")]
        public async Task<IActionResult> SortByPriceDescending() //сортировка по типу цене (повышения)
        {
            try
            {
                var sortedItems = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Price).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по цене (по убыванию): {ex.Message}");
            }
        }

        [HttpGet("SortByQuantityAscending", Name = "SortByQuantityAscending")]
        public async Task<IActionResult> SortByQuantityAscending() //сортировка по типу количеству (уменьшение)
        {
            try
            {
                var sortedItems = await _context.Items.Include(item => item.Type).OrderBy(item => item.Quantity).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по количеству (по возрастанию): {ex.Message}");
            }
        }

        [HttpGet("SortByQuantityDescending", Name = "SortByQuantityDescending")]
        public async Task<IActionResult> SortByQuantityDescending() //сортировка по типу количеству (повышение)
        {
            try
            {
                var sortedItems = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Quantity).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по количеству (по убыванию): {ex.Message}");
            }
        }

        [HttpPost(Name = "PostItems")]
        public async Task<IActionResult> PostItem(string? Name, int Quantity, string? Type, double Price)  // добавление нового предмета в базу данных
        {
            try
            {
                if (string.IsNullOrEmpty(Name) || Quantity <= 0 || string.IsNullOrEmpty(Type))
                    return BadRequest("Невозможно создать предмет из-за неполных данных!");

                Name = Name.Trim();
                Type = Type.Trim();
                var existingType = await _context.TypeOfItems.FirstOrDefaultAsync(t => t.Name == Type);

                if (existingType == null)
                    return BadRequest("Указанного типа предмета не существует в базе данных!");

                Item newItem = new (Name, Quantity, existingType, Price);

                Item result = await _itemService.TryAddItemToDBAsync(newItem);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при добавлении предмета: {ex.Message}");
            }
        }



        [HttpGet("SearchById", Name = "SearchById")]
        public async Task<IActionResult> SearchById(Guid id) // поиск предмета по айди
        {
            try
            {
                var item = await _context.Items.Include(item => item.Type).FirstOrDefaultAsync(i => i.Id == id);
                return item != null ? Ok(item) : NotFound("Предмет не найден!");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }

        [HttpGet("SearchByName", Name = "SearchByName")]
        public async Task<IActionResult> SearchByName(string? Name) // поиск предмета по имени
        {
            if (string.IsNullOrEmpty(Name))
                return BadRequest("Value не может быть Null");

            try
            {
                Name = Name.ToLower().Trim();
                var items = await _context.Items.Include(item => item.Type).Where(i => i.Name != null && i.Name.ToLower() == Name).ToListAsync();
                return items.Count > 0 ? Ok(items) : NotFound($"Предмет с именем '{Name}' не найден.");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }

        [HttpGet("SearchByType", Name = "SearchByType")]
        public async Task<IActionResult> SearchByType(string type) // поиск предмета по типу
        {
            var items = await _context.Items
            .Include(item => item.Type)
            .Where(item => item.Type.Name == type)
            .ToListAsync();

            return items.Any() ? Ok(items) : BadRequest("База не содержит предметов с данным типом!");
        }


        [HttpGet("SearchByPrice", Name = "SearchByPrice")]
        public async Task<IActionResult> SearchByPrice(double? Price) // поиск предмета по цене
        {
            if (Price == null)
                return BadRequest("Заполните поле поиска!");
            try
            {
                var items = await _context.Items.Include(item => item.Type).Where(i => i.Price == Price).ToListAsync();
                return items.Count > 0 ? Ok(items) : BadRequest("База не содержит предметов с данным типом!");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }


        [HttpDelete("DeleteAllItems", Name = "DeleteAllItems")]
        public async Task<IActionResult> DeleteAllItems() // удаления всех предметов из базы данных
        {
            try
            {
                _context.Items.RemoveRange(_context.Items);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при удалении всех предметов: {ex.Message}");
            }
        }
    }
}