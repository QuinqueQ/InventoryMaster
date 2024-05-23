using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Interfaces;
using DomainLayer.Dtos;
using DomainLayer.Entities;

namespace PresentationLayer.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService; //Сервис для работы с Предметами в базе данных

        public ItemsController(IItemService itemService)
        {
            _itemService = itemService;
        }

        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            List<Item>? items = await _itemService.GetItemsAsync();
            if (items == null || items.Count == 0) return NoContent();
            return Ok(items);
        }

        [HttpPut("update")]
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

        [HttpPost]
        public async Task<IActionResult> PostItem(ItemDto itemDto)  // Добавление нового предмета в базу данных
        {
            Item? newItem = await _itemService.PostItemAsync(itemDto);

            if (newItem == null) return BadRequest("Ошибка при создание предмета!");

            return Ok(newItem);
        }

        [HttpDelete("{id}")] // Удаление предмета по айди с выбором колличества удаляемых предметов
        public async Task<IActionResult> DeleteItem(Guid id, int Quantity)
        {
            var selectedItem = await _itemService.DeleteItemAsync(id, Quantity);

            if (selectedItem == null)
                return NotFound(); // Предмет не найден, возвращаем код 404

            if (Quantity >= selectedItem.Quantity)
                return NoContent(); // Предмет успешно удален, возвращаем код 204

            return Ok(selectedItem); // Если удаление частичное, возвращаем предмет с обновленной информацией
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> SearchById(Guid id) // Поиск предмета по айди
        {
            List<Item> Items = await _itemService.SerchAsync(id, ItemFields.Id);
            if (Items.Count == 0)
                return NoContent();

            return Ok(Items);
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> SearchByName(string? name) // Поиск предмета по имени
        {
            List<Item> Items = await _itemService.SerchAsync(name, ItemFields.Name);
            if (Items.Count == 0)
                return NoContent();
            return Ok(Items);
        }

        [HttpGet("{type}")]
        public async Task<IActionResult> SearchByType(string type) // Поиск предмета по типу
        {
            List<Item> Items = await _itemService.SerchAsync(type, ItemFields.Type);
            if (Items.Count == 0)
                return NoContent();
            return Ok(Items);
        }


        [HttpGet("{price}")]
        public async Task<IActionResult> SearchByPrice(double price) // Поиск предмета по цене
        {
            List<Item> Items = await _itemService.SerchAsync(price, ItemFields.Price);
            if (Items.Count == 0)
                return NoContent();
            return Ok(Items);
        }


        [HttpGet("sort-by-name-ascending")]
        public async Task<IActionResult> SortByNameAscending() // Сортировка имени по алфавиту
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Name, false);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("sort-by-name-descending")]
        public async Task<IActionResult> SortByNameDescending() // Сортировка имени по алфавиту(наоборот)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Name, true);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("sort-by-type")]
        public async Task<IActionResult> SortByType() //Сортировка по типу предметов
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Type, false);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("sort-by-price-ascending")]
        public async Task<IActionResult> SortByPriceAscending() //Сортировка по типу цене (уменьшение)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Price, false);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("sort-by-price-descending")]
        public async Task<IActionResult> SortByPriceDescending() //Сортировка по типу цене (повышения)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Price, true);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("sort-by-quantity-ascending")]
        public async Task<IActionResult> SortByQuantityAscending() //Сортировка по типу количеству (уменьшение)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Quantity, false);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("sort-by-quantity-descending")]
        public async Task<IActionResult> SortByQuantityDescending() //Сортировка по типу количеству (повышение)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Quantity, true);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAllItems() // Удаления всех предметов из базы данных
        {
            bool IsItemsDeleted = await _itemService.DeleteAllItemsAsync();
            if (IsItemsDeleted)
                return NoContent();

            return BadRequest("произошла какая то ошибка");
        }
    }
}