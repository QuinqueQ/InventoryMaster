﻿using InventoryMaster.Entities;

namespace InventoryMaster.Model
{
    public class Item
    {
        public Item() //конструктор для генерации уникального айдишника для каждого созданного предмета
        {
            Id = Guid.NewGuid();
        }
        public Item(string Name, int Quantity, TypeOfItems Type, double Price)
        {
            Id = Guid.NewGuid();
            this.Name = Name;
            this.Quantity = Quantity;
            this.TypeOfItemsId = Type.TypeId; // Устанавливаем внешний ключ
            this.Type = Type;
            this.Price = Price;
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public int TypeOfItemsId { get; set; } // Внешний ключ
        public TypeOfItems Type { get; set; } 
        public double Price { get; set; }
    }
}
