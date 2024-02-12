namespace InventoryMaster.Entities
{
    public class TypeOfItems // Класс для типов предмета
    {
        public TypeOfItems(string Name) 
        {
            this.Name = Name;
        }
        public int TypeId { get; set; }
        public string Name { get; set; }
    }
}
