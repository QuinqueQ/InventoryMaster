using InventoryMaster.Model;
using Microsoft.AspNetCore.Mvc;
using InventoryMaster.Enums;
using InventoryMaster.Interfaces;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        public ItemsController(ItemsDBContext context, IItemService itemService)
        {
            _context = context;
            _itemService = itemService;
        }

        private readonly ItemsDBContext _context; //контекст базы данных, через него работает со всеми опирациями
        private readonly IItemService _itemService; // добавил сервис для для добавления предметов в бд

        [HttpGet(Name = "GetItems")]
        public IActionResult Sort(EnumItemSortField Sort) // get items с возможностью сортировки
        {
            if (!_context.Items.Any())
                return NotFound("Ваша база данных пуста!");

            switch (Sort)
            {
                case EnumItemSortField.Name_Ascending:
                    return Ok(_context.Items.OrderBy(item => item.Name));

                case EnumItemSortField.Name_Descending:
                    return Ok(_context.Items.OrderByDescending(item => item.Name));

                case EnumItemSortField.Type:
                    return Ok(_context.Items.OrderBy(item => item.Type));

                case EnumItemSortField.Price_Ascending:
                    return Ok(_context.Items.OrderBy(item => item.Price));

                case EnumItemSortField.Quantity_Ascending:
                    return Ok(_context.Items.OrderBy(item => item.Quantity));

                case EnumItemSortField.Quantity_Descending:
                    return Ok(_context.Items.OrderByDescending(item => item.Quantity));

                case EnumItemSortField.Price_Descending:
                    return Ok(_context.Items.OrderByDescending(item => item.Price));

                default: return Ok(_context.Items);
            }

        }
        [HttpPost(Name = "PostItems")]
        public async Task<IActionResult> Post(string? Name, int Quantity, EnumTypesOFItems Type, double Price)
        {
            if (string.IsNullOrEmpty(Name) || Quantity <= 0 || Type == 0)
                return BadRequest("Невозможно создать предмет из-за неполных данных!");

                Name = Name.Trim();
                Item newItem = new(Name, Quantity, Type, Price);
                try
                {
                    Item result = await _itemService.TryAddItemToDBAsync(newItem);
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Ошибка при добавлении предмета: {ex.Message}");
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
                            return _context.Items.Any(i => i.Id == GuidValue)
                            ? Ok(_context.Items.Where(i => i.Id == GuidValue))
                            : BadRequest("Пусто");
                        }
                    case EnumItemFields.Name:
                        {
                            Value = Value?.ToLower().Trim();
                            List<Item> items = _context.Items.Where(i => i.Name != null && i.Name.ToLower() == Value).ToList();
                            return items.Count > 0 ? Ok(items) : NotFound($"Предмет с именем '{Value}' не найден.");
                        }
                    case EnumItemFields.Type:
                        {
                            return Enum.TryParse(Value, true, out EnumTypesOFItems itemType)
                            ? _context.Items.Any(i => i.Type == itemType)
                            ? Ok(_context.Items.Where(i => i.Type == itemType))
                            : BadRequest("База не содержит предметов с данным типом!")
                            : BadRequest("Такого типа не существует!");
                        }
                    case EnumItemFields.Price:
                        {
                            return double.TryParse(Value, out double DoubleValue)
                            ? _context.Items.Any(i => i.Price == DoubleValue)
                            ? Ok(_context.Items.Where(i => i.Price == DoubleValue))
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
             
            Item? itemToDelete = _context.Items.FirstOrDefault(i => i.Id == Id);

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
