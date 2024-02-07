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
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))] // Этот конвертер преобразует значения перечисления енамки в строки при сериализации в JSON и обратно при десериализации из JSON.
        public EnumTypesOFItems  Type { get; set; } //Енамка типов предметов(пользователю нельзя писать название типа предмета от себя, он должен выбирать из существующего)
        public double Price { get; set; }

    }

}
