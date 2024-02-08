using InventoryMaster.Model;
using Microsoft.AspNetCore.Mvc;
using InventoryMaster.Enums;
using InventoryMaster.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ItemsDBContext _context; //контекст базы данных, через него работает со всеми опирациями
        private readonly IItemService _itemService; // добавил сервис для для добавления предметов в бд

        public ItemsController(ItemsDBContext context, IItemService itemService) // конструктор для контекста бд и сервиса 
        {
            _context = context;
            _itemService = itemService;
        }

        [HttpGet(Name = "GetItems")]
        public async Task<IActionResult> SortDefault()
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            var items = await _context.Items.ToListAsync();
            return Ok(items);
        }

        [HttpGet("SortByNameAscending", Name = "SortByNameAscending")]
        public async Task<IActionResult> SortByNameAscending()
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            var sortedItems = await _context.Items.OrderBy(item => item.Name).ToListAsync();
            return Ok(sortedItems);
        }

        [HttpGet("SortByNameDescending", Name = "SortByNameDescending")]
        public async Task<IActionResult> SortByNameDescending()
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            var sortedItems = await _context.Items.OrderByDescending(item => item.Name).ToListAsync();
            return Ok(sortedItems);
        }

        [HttpGet("SortByType", Name = "SortByType")]
        public async Task<IActionResult> SortByType()
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            var sortedItems = await _context.Items.OrderBy(item => item.Type).ToListAsync();
            return Ok(sortedItems);
        }

        [HttpGet("SortByPriceAscending", Name = "SortByPriceAscending")]
        public async Task<IActionResult> SortByPriceAscending()
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            var sortedItems = await _context.Items.OrderBy(item => item.Price).ToListAsync();
            return Ok(sortedItems);
        }

        [HttpGet("SortByPriceDescending", Name = "SortByPriceDescending")]
        public async Task<IActionResult> SortByPriceDescending()
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            var sortedItems = await _context.Items.OrderByDescending(item => item.Price).ToListAsync();
            return Ok(sortedItems);
        }

        [HttpGet("SortByQuantityAscending", Name = "SortByQuantityAscending")]
        public async Task<IActionResult> SortByQuantityAscending()
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            var sortedItems = await _context.Items.OrderBy(item => item.Quantity).ToListAsync();
            return Ok(sortedItems);
        }

        [HttpGet("SortByQuantityDescending", Name = "SortByQuantityDescending")]
        public async Task<IActionResult> SortByQuantityDescending()
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            var sortedItems = await _context.Items.OrderByDescending(item => item.Quantity).ToListAsync();
            return Ok(sortedItems);
        }

       

        [HttpPost(Name = "PostItems")]
        public async Task<IActionResult> Post(string? Name, int Quantity, EnumTypesOFItems Type, double Price)
        {
            if (string.IsNullOrEmpty(Name) || Quantity <= 0 || Type == 0)
                return BadRequest("Невозможно создать предмет из-за неполных данных!");

            Name = Name.Trim();
            Item newItem = new(Name, Quantity, Type, Price);
            try
            {
                Item result = await _itemService.TryAddItemToDBAsync(newItem);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при добавлении предмета: {ex.Message}");
            }
        }


        [HttpGet("SearchById", Name = "SearchById")]
        public async Task<IActionResult> SearchById(Guid id)
        {
            try
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
                return item != null ? Ok(item) : NotFound("Предмет не найден!");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }

        [HttpGet("SearchByName", Name = "SearchByName")]
        public async Task<IActionResult> SearchByName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return BadRequest("Value не может быть Null");

            try
            {
                value = value.ToLower().Trim();
                var items = await _context.Items.Where(i => i.Name != null && i.Name.ToLower() == value).ToListAsync();
                return items.Count > 0 ? Ok(items) : NotFound($"Предмет с именем '{value}' не найден.");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }

        [HttpGet("SearchByType", Name = "SearchByType")]
        public async Task<IActionResult> SearchByType(string value)
        {
            if (string.IsNullOrEmpty(value))
                return BadRequest("Value не может быть Null");

            try
            {
                if (Enum.TryParse(value, true, out EnumTypesOFItems itemType))
                {
                    var items = await _context.Items.Where(i => i.Type == itemType).ToListAsync();
                    return items.Count > 0 ? Ok(items) : BadRequest("База не содержит предметов с данным типом!");
                }
                return BadRequest("Такого типа не существует!");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }

        [HttpGet("SearchByPrice", Name = "SearchByPrice")]
        public async Task<IActionResult> SearchByPrice(double value)
        {
            try
            {
                var items = await _context.Items.Where(i => i.Price == value).ToListAsync();
                return items.Count > 0 ? Ok(items) : BadRequest("База не содержит предметов с данным типом!");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }




        [HttpDelete(Name = "DeleteItem")] // запрос на удаление предмета из бд, с возможностью выбора колличества
        public async Task<IActionResult> DeleteItem(Guid Id, int Quantity)
        {
            if (Quantity <= 0)
                return BadRequest("Неверно указано количество!");

            Item? itemToDelete = await _context.Items.FirstOrDefaultAsync(i => i.Id == Id);

            if (itemToDelete == null)
                return NotFound("Предмет не найден!");

            itemToDelete.Quantity -= Quantity; // Уменьшаем количество 

            if (itemToDelete.Quantity <= 0)
            {
                _context.Items.Remove(itemToDelete);   // yдаляем предмет из базы данных, если его количество стало меньше или равно нулю
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

        [HttpDelete("DeleteAllItems", Name = "DeleteAllItems")]
        public async Task<IActionResult> DeleteAllItems() // запрос на удаления всех предметов из бд (дев шняга)
        {
            _context.Items.RemoveRange(_context.Items);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
