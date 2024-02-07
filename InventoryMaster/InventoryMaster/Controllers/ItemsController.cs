using InventoryMaster.Model;
using Microsoft.AspNetCore.Mvc;
using InventoryMaster.Enums;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        public ItemsController(ItemsDBContext context)
        {
            _context = context;
            ListItemsFromDB = _context.Items.ToList();    //создаем контекст базы данных и присваиваем нашему листу
        }

        private readonly ItemsDBContext _context;
        private List<Item> ListItemsFromDB { get; set; }//лист базы данных
 
        private IActionResult? TryAddItemToDB(Item newItem) // метод для проверки существует ли похожий предмет, если да, то он его нахолит и меняет колличество (решил оставть в контроллере, потому что за его пределами использование метода я не рассматриваю!!!)
        {

            Item? existingItem = _context.Items.FirstOrDefault(item =>
                item.Type == newItem.Type &&
                item.Name == newItem.Name &&
                item.Price == newItem.Price);

            if (existingItem != null)
            {

                existingItem.Quantity += newItem.Quantity;
                _context.SaveChanges();
                return Ok(existingItem);
            }

            return null;
        }


        [HttpGet(Name = "GetItems")]
        public IActionResult Sort(EnumItemSortField Sort) // get items с возможностью сортировки
        {
            switch (Sort)
            {
                case EnumItemSortField.Name_Ascending:
                    return Ok(ListItemsFromDB.OrderBy(item => item.Name));

                case EnumItemSortField.Name_Descending:
                    return Ok(ListItemsFromDB.OrderByDescending(item => item.Name));

                case EnumItemSortField.Type:
                    return Ok(ListItemsFromDB.OrderBy(item => item.Type));

                case EnumItemSortField.Price_Ascending:
                    return Ok(ListItemsFromDB.OrderBy(item => item.Price));

                case EnumItemSortField.Quantity_Ascending:
                    return Ok(ListItemsFromDB.OrderBy(item => item.Quantity));

                case EnumItemSortField.Quantity_Descending:
                    return Ok(ListItemsFromDB.OrderByDescending(item => item.Quantity));

                case EnumItemSortField.Price_Descending:
                    return Ok(ListItemsFromDB.OrderByDescending(item => item.Price));

                default: return ListItemsFromDB.Any() ? Ok(ListItemsFromDB) : BadRequest("Ваша база данных пуста!");


            }

        }

        [HttpPost(Name = "PostItems")]
        public IActionResult Post(string? Name, int Quantity, EnumTypesOFItems Type, double Price) // пост запрос, для добавления предмета
        {
            
            if (string.IsNullOrEmpty(Name) || Quantity <= 0 || Type == 0)
            {
                return BadRequest("Невозможно создать предмет из-за неполных данных!");
            }
            else
            {
                Name = Name.Trim();
                Item newItem = new(Name, Quantity, Type, Price);

                IActionResult? result = TryAddItemToDB(newItem);

                if (result != null)
                {
                   return result;
                }
                else
                {
                   _context.Items.Add(newItem);
                   _context.SaveChanges();
                   return Ok(newItem);
                }
            }
        }


        [HttpGet("Search/", Name = "GetItemSearch")]// поиск предметов по конкретному полю
        public IActionResult Search(EnumItemFields SearchField, string? Value)
        {
            if (string.IsNullOrEmpty(Value))
                return BadRequest("Value не может быть Null");
            try
            {
                switch (SearchField)
                {
                    case EnumItemFields.Id:
                        {
                            Guid GuidValue = Guid.Parse(Value);
                            return ListItemsFromDB.Any(i => i.Id == GuidValue)
                            ? Ok(ListItemsFromDB.Where(i => i.Id == GuidValue))
                            : BadRequest("Пусто");
                        }
                    case EnumItemFields.Name:
                        {
                            Value = Value?.ToLower().Trim();
                            List<Item> items = ListItemsFromDB.Where(i => i.Name?.ToLower() == Value).ToList();
                            return items.Any() ? Ok(items) : NotFound($"Предмет с именем '{Value}' не найден.");
                        }
                    case EnumItemFields.Type:
                        {
                            return Enum.TryParse(Value, true, out EnumTypesOFItems itemType)
                            ? ListItemsFromDB.Any(i => i.Type == itemType)
                            ? Ok(ListItemsFromDB.Where(i => i.Type == itemType))
                            : BadRequest("База не содержит предметов с данным типом!")
                            : BadRequest("Такого типа не существует!");
                        }
                    case EnumItemFields.Price:
                        {
                            return double.TryParse(Value, out double DoubleValue)
                            ? ListItemsFromDB.Any(i => i.Price == DoubleValue)
                            ? Ok(ListItemsFromDB.Where(i => i.Price == DoubleValue))
                            : BadRequest("Пусто")
                            : BadRequest("Вы ввели неверное значение!");
                        }
                    default: return BadRequest("Укажите поле поиска!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Произошла ошибка " + ex.Message);
            }
        }



        [HttpDelete(Name = "DeleteItem")] // запрос на удаление предмета из бд, с возможностью выбора колличества
        public IActionResult DeleteItem(Guid Id, int Quantity)
        {
            if (Quantity <= 0)
                return BadRequest("Неверно указано количество!");
             
            Item? itemToDelete = ListItemsFromDB.FirstOrDefault(i => i.Id == Id);

            if (itemToDelete == null)
                return NotFound("Предмет не найден!"); 

            itemToDelete.Quantity -= Quantity; // Уменьшаем количество 

            if (itemToDelete.Quantity <= 0)
            {
                // yдаляем предмет из базы данных, если его количество стало меньше или равно нулю
                _context.Items.Remove(itemToDelete);
                _context.SaveChanges();
                return Ok("Предмет успешно удален !");
            }
            else
            {
                _context.Items.Update(itemToDelete);
                _context.SaveChanges();
            }
            return Ok(itemToDelete);
        }


        [HttpDelete("DeleteAllItems/",Name = "DeleteAllItems")]
        public IActionResult DeleteAllItems() // запрос на удаления всех предметов из бд (дев шняга)
        {
            _context.Items.RemoveRange(_context.Items);
            _context.SaveChanges();
            return Ok("Все записи успешно удалены из базы данных.");
        }




    }
}
