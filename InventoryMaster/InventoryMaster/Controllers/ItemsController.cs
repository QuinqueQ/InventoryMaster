using InventoryMaster.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private static  List<Item> ListOfItems = new ()
{
            new Item { Name = "Cok", Price = 12.3, Type = TypesOFItems.Liquid },
            new Item { Name = "Chocolate", Price = 23.5, Type = TypesOFItems.Eat },
            new Item { Name = "Water", Price = 1.5, Type = TypesOFItems.Liquid },
            new Item { Name = "Bread", Price = 5.0, Type = TypesOFItems.Eat },
            new Item { Name = "Juice", Price = 3.0, Type = TypesOFItems.Liquid },
            new Item { Name = "Pizza", Price = 15.5, Type = TypesOFItems.Eat },
            new Item { Name = "Milk", Price = 2.5, Type = TypesOFItems.Liquid },
            new Item { Name = "Cheese", Price = 8.0, Type = TypesOFItems.Eat },
            new Item { Name = "Soda", Price = 1.0, Type = TypesOFItems.Liquid },
            new Item { Name = "Chips", Price = 4.5, Type = TypesOFItems.Eat },
            new Item { Name = "Tea", Price = 2.0, Type = TypesOFItems.Liquid },
            new Item { Name = "Ice Cream", Price = 6.5, Type = TypesOFItems.Eat },
            new Item { Name = "Ice Tea", Price = 5.5, Type = TypesOFItems.Liquid }
};

        
        

        [HttpGet(Name = "GetItems")]
        public IActionResult Get()//основной гет запрос на все существующие предметы в нашем "хранилище"
        {
            if (ListOfItems == null || ListOfItems.Count == 0)
            {
                return BadRequest("Ваш инвентарь пуст :(");
            }
            return Ok(ListOfItems);
        }

        [HttpPost(Name = "PostItems")]// пост запрос, для добавления предмета
        public IActionResult Post(string? Name, int Quantity, TypesOFItems Type, double Price)
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
                            if (Enum.TryParse(Value, true, out TypesOFItems itemType))
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
