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
        private readonly ItemsDBContext _context; // Контекст бд
        private readonly IItemService _itemService; //Подключаю сервис для добавления предметов (он работает с колличеством предметов)

        public ItemsController(ItemsDBContext context, IItemService itemService)
        {
            _context = context;
            _itemService = itemService;
        }

        [HttpGet(Name = "GetItems")]
        public async Task<IActionResult> GetItems()
        {
            List<Item>? items = await _itemService.GetItemsAsync();
            if (items == null || items.Count == 0 ) return NoContent();
            return Ok(items);
        }

        [HttpPut("UpdateItem", Name = "UpdateItem")]
        [ProducesResponseType(typeof(Item), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ItemDto itemUpdateDto)
        {
           var UpdItem = await _itemService.UpdateItemAsync(id, itemUpdateDto);

            if (UpdItem == null)
                return NoContent();

            return Ok(UpdItem);

        }

        [HttpPost(Name = "PostItems")]
        public async Task<IActionResult> PostItem(ItemDto itemDto)  // Добавление нового предмета в базу данных
        {
            Item? newItem = await _itemService.PostItem(itemDto);

            if (newItem == null) return BadRequest("Ошибка при создание предмета!");

            return Ok(newItem);
        }

        [HttpDelete(Name = "DeleteItem")] // Удаление предмета по айди с выбором колличества удаляемых предметов
        public async Task<IActionResult> DeleteItem(Guid Id, int Quantity)
        {
            var selectedItem = await _itemService.DeleteItemAsync(Id, Quantity);

            if (selectedItem == null)
                return NotFound(); // Предмет не найден, возвращаем код 404

            if (Quantity >= selectedItem.Quantity)
                return NoContent(); // Предмет успешно удален, возвращаем код 204

            return Ok(selectedItem); // Если удаление частичное, возвращаем предмет с обновленной информацией
        }


        [HttpGet("SortByNameAscending", Name = "SortByNameAscending")]
        public async Task<IActionResult> SortByNameAscending() // Сортировка имени по алфавиту
        {
            try
            {
                List<Item> sortedItems = await _context.Items.Include(item => item.Type).OrderBy(item => item.Name).ToListAsync();

                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по имени (по возрастанию): {ex.Message}");
            }
        }

        [HttpGet("SortByNameDescending", Name = "SortByNameDescending")]
        public async Task<IActionResult> SortByNameDescending() // Сортировка имени по алфавиту(наоборот)
        {
            try
            {
                List<Item> sortedItems = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Name).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по имени (по убыванию): {ex.Message}");
            }
        }

        [HttpGet("SortByType", Name = "SortByType")]
        public async Task<IActionResult> SortByType() //Сортировка по типу предметов
        {
            try
            {
                List<Item> sortedItems = await _context.Items.Include(item => item.Type).OrderBy(item => item.Type).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по типу: {ex.Message}");
            }
        }

        [HttpGet("SortByPriceAscending", Name = "SortByPriceAscending")]
        public async Task<IActionResult> SortByPriceAscending() //Сортировка по типу цене (уменьшение)
        {
            try
            {
                List<Item> sortedItems = await _context.Items.Include(item => item.Type).OrderBy(item => item.Price).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по цене (по возрастанию): {ex.Message}");
            }
        }

        [HttpGet("SortByPriceDescending", Name = "SortByPriceDescending")]
        public async Task<IActionResult> SortByPriceDescending() //Сортировка по типу цене (повышения)
        {
            try
            {
                List<Item> sortedItems = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Price).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по цене (по убыванию): {ex.Message}");
            }
        }

        [HttpGet("SortByQuantityAscending", Name = "SortByQuantityAscending")]
        public async Task<IActionResult> SortByQuantityAscending() //Сортировка по типу количеству (уменьшение)
        {
            try
            {
                List<Item> sortedItems = await _context.Items.Include(item => item.Type).OrderBy(item => item.Quantity).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по количеству (по возрастанию): {ex.Message}");
            }
        }

        [HttpGet("SortByQuantityDescending", Name = "SortByQuantityDescending")]
        public async Task<IActionResult> SortByQuantityDescending() //Сортировка по типу количеству (повышение)
        {
            try
            {
                List<Item> sortedItems = await _context.Items.Include(item => item.Type).OrderByDescending(item => item.Quantity).ToListAsync();
                return Ok(sortedItems);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при сортировке предметов по количеству (по убыванию): {ex.Message}");
            }
        }


        [HttpGet("SearchById", Name = "SearchById")]
        public async Task<IActionResult> SearchById(Guid id) // Поиск предмета по айди
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
        public async Task<IActionResult> SearchByName(string? Name) // Поиск предмета по имени
        {
            if (string.IsNullOrWhiteSpace(Name))
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
        public async Task<IActionResult> SearchByType(string type) // Поиск предмета по типу
        {
            var items = await _context.Items
            .Include(item => item.Type)
            .Where(item => item.Type.Name == type)
            .ToListAsync();

            return items.Any() ? Ok(items) : BadRequest("База не содержит предметов с данным типом!");
        }


        [HttpGet("SearchByPrice", Name = "SearchByPrice")]
        public async Task<IActionResult> SearchByPrice(double Price) // Поиск предмета по цене
        {
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
        public async Task<IActionResult> DeleteAllItems() // Удаления всех предметов из базы данных
        {
            bool IsItemsDeleted = await _itemService.DeleteAllItems();
            if (IsItemsDeleted)
                return NoContent();

            return BadRequest("произошла какая то ошибка");
        }
    }
}