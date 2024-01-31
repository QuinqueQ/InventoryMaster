using InventoryMaster.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.Collections.Generic;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        public static  List<Item> items = new List<Item>
{
            new Item { Id = 1, Name = "Cok", Price = 12.3, Type = TypesOFItems.Liquid },
            new Item { Id = 2, Name = "Chocolate", Price = 23.5, Type = TypesOFItems.Eat },
            new Item { Id = 3, Name = "Water", Price = 1.5, Type = TypesOFItems.Liquid },
            new Item { Id = 4, Name = "Bread", Price = 5.0, Type = TypesOFItems.Eat },
            new Item { Id = 5, Name = "Juice", Price = 3.0, Type = TypesOFItems.Liquid },
            new Item { Id = 6, Name = "Pizza", Price = 15.5, Type = TypesOFItems.Eat },
            new Item { Id = 7, Name = "Milk", Price = 2.5, Type = TypesOFItems.Liquid },
            new Item { Id = 8, Name = "Cheese", Price = 8.0, Type = TypesOFItems.Eat },
            new Item { Id = 9, Name = "Soda", Price = 1.0, Type = TypesOFItems.Liquid },
            new Item { Id = 10, Name = "Chips", Price = 4.5, Type = TypesOFItems.Eat },
            new Item { Id = 11, Name = "Tea", Price = 2.0, Type = TypesOFItems.Liquid },
            new Item { Id = 12, Name = "Ice Cream", Price = 6.5, Type = TypesOFItems.Eat }
};

        [HttpGet(Name = "GetItems")]
        public IEnumerable<Item> Get()
        {

            return items;
        }

        [HttpPost (Name = "PostItems")]
        public void  Post(Item item)
        {
            items.Add(item);
 
        }

        [HttpGet("GetSearch/", Name = "GetItemSearch")]
        public IEnumerable<Item> Search(string TypeOfSearch, string Value)
        {
            switch (TypeOfSearch)
            {
                case "Id":
                    {
                        try
                        {

                            int Intvalue = int.Parse(Value);
                            IEnumerable<Item> SortedListItems = items.Where(i => i.Id == Intvalue);
                            return SortedListItems;
                        }
                        catch { return items; }; 
                    }
                case "Name":
                    {
                        return items.Where(i => i.Name == Value);
                    }
                case "Type":
                    {
                        if (Enum.TryParse(Value, out TypesOFItems itemType))
                        {
                            IEnumerable<Item> SortedListItems = items.Where(i => i.Type == itemType);
                            return SortedListItems;
                        }
                        else
                        {
                            // Обработка случая, когда Value не может быть преобразовано в TypesOFItems
                            return items;
                        }
                    }
                case "Price":
                    {
                        try
                        {
                            int Intvalue = int.Parse(Value);
                            IEnumerable<Item> SortedListItems = items.Where(i => i.Price <= Intvalue);
                            return SortedListItems;
                        }
                        catch { return items; };
                    }

                default: return items;


            }
        }


    }
}
