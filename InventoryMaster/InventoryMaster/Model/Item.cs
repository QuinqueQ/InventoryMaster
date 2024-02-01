using System.Text.Json.Serialization;
using System.Linq;
namespace InventoryMaster.Model
{
    public class Item
    {
        public Item()
        {
            Id = Guid.NewGuid();
        }
        public Item(string Name, int Quantity, TypesOFItems Type, double Price)
        {
            Id = Guid.NewGuid();
            this.Name = Name;
            this.Quantity = Quantity;
            this.Type = Type;
            this.Price = Price;
        }
        public Guid Id { get; }
        public string? Name { get; set; }
        public int Quantity { get; set; } = 1;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TypesOFItems Type { get; set; }
        public string TypeName => Type.ToString();
        public double Price { get; set; }

        public void AddItemsInList(List<Item> ListOfItems, Item NewItem)
        {
          
            foreach (var item in ListOfItems)
            {
                if (item.Type == NewItem.Type && item.Name == NewItem.Name && item.Price == NewItem.Price)
                {
                    item.Quantity = item.Quantity + NewItem.Quantity;
                    return;
                }
            }
            ListOfItems.Add(NewItem);

        }


    }

}
