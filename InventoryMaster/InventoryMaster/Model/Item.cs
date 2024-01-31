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
        public Guid Id { get; }
        public string Name { get; set; }
        public int count { get; set; } = 0;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TypesOFItems Type { get; set; }
        public double Price { get; set; }



        
    }

}
