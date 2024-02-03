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

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private static  List<Item> ListOfItems = new ()
{
            new Item { Name = "Cok", Price = 12.3, Type = EnumTypesOFItems.Liquid },
            new Item { Name = "Chocolate", Price = 23.5, Type = EnumTypesOFItems.Eat },
            new Item { Name = "Water", Price = 1.5, Type = EnumTypesOFItems.Liquid },
            new Item { Name = "Bread", Price = 5.0, Type = EnumTypesOFItems.Eat },
            new Item { Name = "Juice", Price = 3.0, Type = EnumTypesOFItems.Liquid },
            new Item { Name = "Pizza", Price = 15.5, Type = EnumTypesOFItems.Eat },
            new Item { Name = "Milk", Price = 2.5, Type = EnumTypesOFItems.Liquid },
            new Item { Name = "Cheese", Price = 8.0, Type = EnumTypesOFItems.Eat },
            new Item { Name = "Soda", Price = 1.0, Type = EnumTypesOFItems.Liquid },
            new Item { Name = "Chips", Price = 4.5, Type = EnumTypesOFItems.Eat },
            new Item { Name = "Tea", Price = 2.0, Type = EnumTypesOFItems.Liquid },
            new Item { Name = "Ice Cream", Price = 6.5, Type = EnumTypesOFItems.Eat },
            new Item { Name = "Ice Tea", Price = 5.5, Type = EnumTypesOFItems.Liquid }
};




        [HttpGet(Name = "GetItems")]
        public IActionResult Sort(EnumItemSortField Sort)
        {
            switch (Sort)
            {
                case EnumItemSortField.Name_Ascending:
                    return Ok(ListOfItems.OrderBy(item => item.Name));

                case EnumItemSortField.Name_Descending: 
                    return Ok(ListOfItems.OrderByDescending(item => item.Name));

                case EnumItemSortField.Type:
                    return Ok(ListOfItems.OrderBy(item => item.Type));

                case EnumItemSortField.Price_Ascending:
                    return Ok(ListOfItems.OrderBy(item => item.Price));

                case EnumItemSortField.Price_Descending:
                    return Ok(ListOfItems.OrderByDescending(item => item.Price));

                default: return Ok(ListOfItems); 
            }

        }

        [HttpDelete("Id", Name = "DeleteItem")]
        public IActionResult DeleteQuantity(Guid Id)
        {
            try
            {
                Item itemToRemove = ListOfItems.FirstOrDefault(item => item.Id == Id);

                if (itemToRemove == null)
                {
                    return NotFound("Предмет не найден");
                }

                ListOfItems.Remove(itemToRemove);
                return Ok("Предмет успешно удален");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }


        [HttpPut("UpdateItem", Name = "UpdateItem")]
        public IActionResult UpdateItem(Guid ItemId, [FromBody] Item updatedItem)
        {
            try
            {
                Item itemToUpdate = ListOfItems.FirstOrDefault(item => item.Id == ItemId);

                if (itemToUpdate == null)
                {
                    return NotFound("Предмет не найден");
                }

               
                itemToUpdate.Name = updatedItem.Name;
                itemToUpdate.Quantity = updatedItem.Quantity;
                itemToUpdate.Type = updatedItem.Type;
                itemToUpdate.Price = updatedItem.Price;

                return Ok("Предмет успешно обновлен");
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }


        [HttpPost(Name = "PostItems")]// пост запрос, для добавления предмета
        public IActionResult Post(string? Name, int Quantity, EnumTypesOFItems Type, double Price)
        {
            try
            {
                if (string.IsNullOrEmpty(Name))
                    return BadRequest("Невозможно создать предмет без названия!");

                else if (Quantity == 0)
                    return BadRequest("Невозможно создать предмет с колличество 0!");

                else if (Type == 0)
                    return BadRequest("Невозможно создать предмет не указав его тип!");

                else
                {
                    Item item = new (Name, Quantity, Type, Price);
                    item.AddItemsInList(ListOfItems, item);
                    return Ok(item);
                }
            }
            catch { return BadRequest("Вы Указали недопустимые значения"); }

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
                            IEnumerable<Item> SortedListItems = ListOfItems.Where(i => i.Id == GuidValue);
                            return Ok(SortedListItems);
                        }
                    case EnumItemFields.Name:
                        {
                            Value = Value.ToLower().Trim();
                            return Ok(ListOfItems.Where(i => i.Name?.ToLower() == Value));
                        }
                    case EnumItemFields.Type:
                        {
                            if (Enum.TryParse(Value, true, out EnumTypesOFItems itemType))
                            {
                                IEnumerable<Item> sortedListItems = ListOfItems.Where(i => i.Type == itemType);
                                return Ok(sortedListItems);
                            }
                            else { return BadRequest("Такого типа не существует!"); }
                        }
                    case EnumItemFields.Price:
                        {
                            if (double.TryParse(Value, out double DoubleValue))
                            {
                                IEnumerable<Item> SortedListItems = ListOfItems.Where(i => i.Price == DoubleValue);
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
