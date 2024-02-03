using System.Text.Json.Serialization;
using System.Linq;
using InventoryMaster.Enums;
namespace InventoryMaster.Model
{
    public class Item
    {
        public Item() //конструктор для генерации уникального айдишника для каждого созданного предмета
        {
            Id = Guid.NewGuid();
        }
        public Item(string Name, int Quantity, EnumTypesOFItems Type, double Price)
        {
            Id = Guid.NewGuid();
            this.Name = Name;
            this.Quantity = Quantity;
            this.Type = Type;
            this.Price = Price;
        }
        public Guid Id { get; }
        public string? Name { get; set; }
        public int Quantity { get; set; } = 1; // сразу присваиваю колличество 1, в основном для создания предмета хардкодом(чтобы не создавался предмет с колличеством 0)

        [JsonConverter(typeof(JsonStringEnumConverter))] // Этот конвертер преобразует значения перечисления енамки в строки при сериализации в JSON и обратно при десериализации из JSON.
        public EnumTypesOFItems Type { get; set; } //Енамка типов предметов(пользователю нельзя писать название типа предмета от себя, он должен выбирать из существующего или же, сначала он должен будет добавлять в енамку новый тип предметов)
        public double Price { get; set; }

        public void AddItemsInList(List<Item> ListOfItems, Item NewItem) // это метод который мы используем для добавления нового предмета в наше "хранилище", он работает с колличеством одинаковых предметов
        {
            NewItem.Name = NewItem.Name?.Trim();
            foreach (var item in ListOfItems)
            {
                if (item.Type == NewItem.Type && item.Name == NewItem.Name && item.Price == NewItem.Price)
                {
                    item.Quantity += NewItem.Quantity;
                    return;
                }
            }
            ListOfItems.Add(NewItem);

        }


    }

}
