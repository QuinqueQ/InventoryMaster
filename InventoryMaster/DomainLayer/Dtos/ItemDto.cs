namespace DomainLayer.Dtos
{
    public class ItemDto // DTO класс, для обновления предмета, что-то вроде шаблона без ненужных полей для заполнения
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string TypeName { get; set; }
        public double Price { get; set; }
    }
}
