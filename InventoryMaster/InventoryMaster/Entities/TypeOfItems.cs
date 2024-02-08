namespace InventoryMaster.Entities
{
    public class TypeOfItems
    {
        public TypeOfItems(string Name) // класс для типов предмета
        {
            this.Name = Name;
        }
        public int TypeId { get; set; }
        public string Name { get; set; }
    }
}
