using InventoryMaster.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using InventoryMaster.Enums;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        public ItemsController(ItemsDBContext context)
        {
            _context = context;
            ItemsFromDatabase = _context.Items.ToList();
        }

        private readonly ItemsDBContext _context;
        private List<Item> ItemsFromDatabase { get; set; }//лист базы данных
 
        private IActionResult? TryUpdateExistingItem(Item newItem) // метод для проверки существует ли похожий предмет, если да, то он его нахолит и меняет колличество (решил оставть в контроллере, потому что за его пределами использование метода я не рассматриваю!!!)
        {

            var existingItem = _context.Items.FirstOrDefault(item =>
                item.Type == newItem.Type &&
                item.Name == newItem.Name &&
                item.Price == newItem.Price);

            if (existingItem != null)
            {

                existingItem.Quantity += newItem.Quantity;
                _context.SaveChanges();
                return Ok(existingItem);
            }

            return null;
        }


        [HttpGet(Name = "GetItems")]
        public IActionResult Sort(EnumItemSortField Sort) // get items с возможностью сортировки
        {
            switch (Sort)
            {
                case EnumItemSortField.Name_Ascending:
                    return Ok(ItemsFromDatabase.OrderBy(item => item.Name));

                case EnumItemSortField.Name_Descending:
                    return Ok(ItemsFromDatabase.OrderByDescending(item => item.Name));

                case EnumItemSortField.Type:
                    return Ok(ItemsFromDatabase.OrderBy(item => item.Type));

                case EnumItemSortField.Price_Ascending:
                    return Ok(ItemsFromDatabase.OrderBy(item => item.Price));

                case EnumItemSortField.Price_Descending:
                    return Ok(ItemsFromDatabase.OrderByDescending(item => item.Price));

                default: return Ok(ItemsFromDatabase);
            }

        }

        [HttpPut("UpdateItem/{itemId}", Name = "UpdateItem")]
        public IActionResult UpdateItem(Guid itemId, [FromBody] Item updatedItem)
        {
            try
            {
                Item itemToUpdate = _context.Items.FirstOrDefault(item => item.Id == itemId);

                if (itemToUpdate == null)
                {
                    return NotFound("Предмет не найден");
                }

                // Проверяем, какое поле нужно обновить
                if (updatedItem.Name != null)
                {
                    itemToUpdate.Name = updatedItem.Name;
                }
                else if (updatedItem.Quantity != 0)
                {
                    itemToUpdate.Quantity = updatedItem.Quantity;
                }
                else if (updatedItem.Type != EnumTypesOFItems.None)
                {
                    itemToUpdate.Type = updatedItem.Type;
                }
                else if (updatedItem.Price != 0)
                {
                    itemToUpdate.Price = updatedItem.Price;
                }

                _context.SaveChanges();

                return Ok("Предмет успешно обновлен");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }


        [HttpDelete("{id}", Name = "DeleteItem")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                // Находим элемент в базе данных по его идентификатору
                Item itemToRemove = _context.Items.FirstOrDefault(item => item.Id == id);

                // Проверяем, существует ли элемент с таким идентификатором
                if (itemToRemove == null)
                {
                    return NotFound("Предмет не найден");
                }

                // Удаляем элемент из контекста базы данных
                _context.Items.Remove(itemToRemove);

                // Сохраняем изменения в базе данных
                _context.SaveChanges();

                // Возвращаем успешный результат
                return Ok("Предмет успешно удален");
            }
            catch (Exception ex)
            {
                // Если произошла ошибка, возвращаем сообщение об ошибке
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }

        [HttpPost(Name = "PostItems")] 
        public IActionResult Post(string? Name, int Quantity, EnumTypesOFItems Type, double Price) // пост запрос, для добавления предмета
        {
            try
            {
                if (string.IsNullOrEmpty(Name))
                    return BadRequest("Невозможно создать предмет без названия!");

                else if (Quantity == 0)
                    return BadRequest("Невозможно создать предмет с количеством 0!");

                else if (Type == 0)
                    return BadRequest("Невозможно создать предмет не указав его тип!");

                else
                {
                    Item newItem = new(Name, Quantity, Type, Price);

                    IActionResult? result = TryUpdateExistingItem(newItem);

                    if (result != null)
                    {
                        return result;
                    }
                    else
                    {
                        _context.Items.Add(newItem);
                        _context.SaveChanges();
                        return Ok(newItem);
                    }
                }
            }
            catch
            {
                return BadRequest("Вы указали недопустимые значения");
            }
        }


        [HttpGet("Search/", Name = "GetItemSearch")]// поиск предметов по конкретному полю
        public IActionResult Search(EnumItemFields SearchField, string? Value)
        {
            try
            {
                if (string.IsNullOrEmpty(Value))
                    return BadRequest("Value не может быть Null");

                switch (SearchField)
                {
                    case EnumItemFields.Id:
                        {
                            Guid GuidValue = Guid.Parse(Value);
                            IEnumerable<Item> SortedListItems = ItemsFromDatabase.Where(i => i.Id == GuidValue);
                            return Ok(SortedListItems);
                        }
                    case EnumItemFields.Name:
                        {
                            Value = Value?.ToLower().Trim();
                            List<Item> items = ItemsFromDatabase.Where(i => i.Name?.ToLower() == Value).ToList();

                            if (items.Any())
                            {
                                return Ok(items);
                            }
                            else
                            {
                                return NotFound($"Предмет с именем '{Value}' не найден.");
                            }
                        }
                    case EnumItemFields.Type:
                        {
                            if (Enum.TryParse(Value, true, out EnumTypesOFItems itemType))
                            {
                                IEnumerable<Item> sortedListItems = ItemsFromDatabase.Where(i => i.Type == itemType);
                                return Ok(sortedListItems);
                            }
                            else { return BadRequest("Такого типа не существует!"); }
                        }
                    case EnumItemFields.Price:
                        {
                            if (double.TryParse(Value, out double DoubleValue))
                            {
                                IEnumerable<Item> SortedListItems = ItemsFromDatabase.Where(i => i.Price == DoubleValue);
                                return Ok(SortedListItems);
                            }
                            else { return BadRequest("Вы ввели неверное значение!"); }
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
    }
}
