using System.Text.Json.Serialization;

namespace InventoryMaster.Model
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public TypesOFItems Type { get; set; }
        public double Price { get; set; }

    }
}
