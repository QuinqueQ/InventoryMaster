using InventoryMaster.Model;
using Microsoft.AspNetCore.Mvc;
using InventoryMaster.Enums;
using InventoryMaster.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Sort(EnumItemSortField Sort) // get items с возможностью сортировки
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            switch (Sort)
            {
                case EnumItemSortField.Name_Ascending:
                    return Ok(await _context.Items.OrderBy(item => item.Name).ToListAsync());

                case EnumItemSortField.Name_Descending:
                    return Ok(await _context.Items.OrderByDescending(item => item.Name).ToListAsync());

                case EnumItemSortField.Type:
                    return Ok(await _context.Items.OrderBy(item => item.Type).ToListAsync());

                case EnumItemSortField.Price_Ascending:
                    return Ok(await _context.Items.OrderBy(item => item.Price).ToListAsync());

                case EnumItemSortField.Quantity_Ascending:
                    return Ok(await _context.Items.OrderBy(item => item.Quantity).ToListAsync());

                case EnumItemSortField.Quantity_Descending:
                    return Ok(await _context.Items.OrderByDescending(item => item.Quantity).ToListAsync());

                case EnumItemSortField.Price_Descending:
                    return Ok(await _context.Items.OrderByDescending(item => item.Price).ToListAsync());

                default:
                    return Ok(await _context.Items.ToListAsync());
            }
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

        [HttpGet("Search/", Name = "GetItemSearch")]// поиск предметов по конкретному полю
        public async Task<IActionResult> Search(EnumItemFields SearchField, string? Value)
        {
            if (string.IsNullOrEmpty(Value))
                return BadRequest("Value не может быть Null");
            try
            {
                switch (SearchField)
                {
                    case EnumItemFields.Id:
                        {
                            Guid GuidValue = Guid.Parse(Value);
                            return _context.Items.Any(i => i.Id == GuidValue)
                            ? Ok(await _context.Items.Where(i => i.Id == GuidValue).ToListAsync())
                            : BadRequest("База не содержит предметов с данным типом!");
                        }
                    case EnumItemFields.Name:
                        {
                            Value = Value?.ToLower().Trim();
                            List<Item> items = await _context.Items.Where(i => i.Name != null && i.Name.ToLower() == Value).ToListAsync();
                            return items.Count > 0 ? Ok(items) : NotFound($"Предмет с именем '{Value}' не найден.");
                        }
                    case EnumItemFields.Type:
                        {
                            return Enum.TryParse(Value, true, out EnumTypesOFItems itemType)
                            ? _context.Items.Any(i => i.Type == itemType)
                            ? Ok(await _context.Items.Where(i => i.Type == itemType).ToListAsync())
                            : BadRequest("База не содержит предметов с данным типом!")
                            : BadRequest("Такого типа не существует!");
                        }
                    case EnumItemFields.Price:
                        {
                            return double.TryParse(Value, out double DoubleValue)
                            ? _context.Items.Any(i => i.Price == DoubleValue)
                            ? Ok(await _context.Items.Where(i => i.Price == DoubleValue).ToListAsync())
                            : BadRequest("База не содержит предметов с данным типом!")
                            : BadRequest("Вы ввели неверное значение!");
                        }
                    default:
                        return BadRequest("Укажите поле поиска!");
                }
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

        [HttpDelete("DeleteAllItems/", Name = "DeleteAllItems")]
        public async Task<IActionResult> DeleteAllItems() // запрос на удаления всех предметов из бд (дев шняга)
        {
            _context.Items.RemoveRange(_context.Items);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
