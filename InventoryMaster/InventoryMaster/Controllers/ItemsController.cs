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
        public static  List<Item> ListOfItems = new List<Item>
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
        public IEnumerable<Item> Get()
        {

            return ListOfItems;
        }

        [HttpPost (Name = "PostItems")]
        public void  Post(Item item)
        {
            item.AddItemsInList(ListOfItems, item);
           
 
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

                            Guid Intvalue = Guid.Parse(Value);
                            IEnumerable<Item> SortedListItems = ListOfItems.Where(i => i.Id == Intvalue);
                            return SortedListItems;
                        }
                        catch { return ListOfItems; }; 
                    }
                case "Name":
                    {
                        return ListOfItems.Where(i => i.Name == Value);
                    }
                case "Type":
                    {
                        if (Enum.TryParse(Value, out TypesOFItems itemType))
                        {
                            IEnumerable<Item> SortedListItems = ListOfItems.Where(i => i.Type == itemType);
                            return SortedListItems;
                        }
                        else
                        {
                            // Обработка случая, когда Value не может быть преобразовано в TypesOFItems
                            return ListOfItems;
                        }
                    }
                case "Price":
                    {
                        try
                        {
                            int Intvalue = int.Parse(Value);
                            IEnumerable<Item> SortedListItems = ListOfItems.Where(i => i.Price <= Intvalue);
                            return SortedListItems;
                        }
                        catch { return ListOfItems; };
                    }

                default: return ListOfItems;


            }
        }


    }
}
