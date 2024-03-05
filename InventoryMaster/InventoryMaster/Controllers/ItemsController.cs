using InventoryMaster.Model;
using Microsoft.AspNetCore.Mvc;
using InventoryMaster.Interfaces;
using InventoryMaster.Dtos;
using InventoryMaster.Entities;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService; //Сервис для работы с Предметами в базе данных

        public ItemsController(IItemService itemService)
        {
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
            Item? newItem = await _itemService.PostItemAsync(itemDto);

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
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Name, false);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("SortByNameDescending", Name = "SortByNameDescending")]
        public async Task<IActionResult> SortByNameDescending() // Сортировка имени по алфавиту(наоборот)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Name, true);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("SortByType", Name = "SortByType")]
        public async Task<IActionResult> SortByType() //Сортировка по типу предметов
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Type, false);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("SortByPriceAscending", Name = "SortByPriceAscending")]
        public async Task<IActionResult> SortByPriceAscending() //Сортировка по типу цене (уменьшение)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Price, false);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("SortByPriceDescending", Name = "SortByPriceDescending")]
        public async Task<IActionResult> SortByPriceDescending() //Сортировка по типу цене (повышения)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Price, true);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("SortByQuantityAscending", Name = "SortByQuantityAscending")]
        public async Task<IActionResult> SortByQuantityAscending() //Сортировка по типу количеству (уменьшение)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Quantity, false);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }

        [HttpGet("SortByQuantityDescending", Name = "SortByQuantityDescending")]
        public async Task<IActionResult> SortByQuantityDescending() //Сортировка по типу количеству (повышение)
        {
            List<Item> SortedList = await _itemService.SortAsync(ItemFields.Quantity, true);
            if (SortedList.Count == 0)
                return NoContent();
            return Ok(SortedList);
        }


        [HttpGet("SearchById", Name = "SearchById")]
        public async Task<IActionResult> SearchById(Guid Id) // Поиск предмета по айди
        {
          List<Item> Items = await _itemService.SerchAsync(Id, ItemFields.Id);
            if (Items.Count == 0)
                return NoContent();

            return Ok(Items);
        }

        [HttpGet("SearchByName", Name = "SearchByName")]
        public async Task<IActionResult> SearchByName(string? Name) // Поиск предмета по имени
        {
           List<Item> Items = await _itemService.SerchAsync(Name, ItemFields.Name);
            if (Items.Count == 0)
                return NoContent();
            return Ok(Items);
        }

        [HttpGet("SearchByType", Name = "SearchByType")]
        public async Task<IActionResult> SearchByType(string Type) // Поиск предмета по типу
        {
            List<Item> Items = await _itemService.SerchAsync(Type, ItemFields.Type);
            if (Items.Count == 0)
                return NoContent();
            return Ok(Items);
        }


        [HttpGet("SearchByPrice", Name = "SearchByPrice")]
        public async Task<IActionResult> SearchByPrice(double Price) // Поиск предмета по цене
        {
            List<Item> Items = await _itemService.SerchAsync(Price, ItemFields.Price);
            if (Items.Count == 0)
                return NoContent();
            return Ok(Items);
        }


        [HttpDelete("DeleteAllItems", Name = "DeleteAllItems")]
        public async Task<IActionResult> DeleteAllItems() // Удаления всех предметов из базы данных
        {
            bool IsItemsDeleted = await _itemService.DeleteAllItemsAsync();
            if (IsItemsDeleted)
                return NoContent();

            return BadRequest("произошла какая то ошибка");
        }
    }
}