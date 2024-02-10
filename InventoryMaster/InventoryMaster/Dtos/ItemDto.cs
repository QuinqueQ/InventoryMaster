using InventoryMaster.Entities;

namespace InventoryMaster.Dtos
{
    public class ItemDto // DTO класс, для обновления предмета, что то вроде шаблона без ненужных данных для заполнения
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Type { get; set; }
        public double Price { get; set; }
    }
}
