using InventoryMaster.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("api/type-of-items")]
    public class TypeOfItemsController : ControllerBase
    {
        private readonly ITypeOfItemService _typeOfItemService; // сервис для работы с предметами типов из базы данных

        public TypeOfItemsController(ITypeOfItemService typeOfItemService)
        {
            _typeOfItemService = typeOfItemService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()  // Получаем все типы предметов
        {
           var typeOfItems = await _typeOfItemService.GetTypeOfItemsAsync();
            return typeOfItems == null ? NoContent() : Ok(typeOfItems);
        }


        [HttpPost]
        public async Task<IActionResult> PostType(string? TypeName) // Создание нового типа предмета
        {
            var typeOfItems = await _typeOfItemService.PostTypeAsync(TypeName);
            return typeOfItems == null ? NoContent() : Ok(typeOfItems);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTypeById(int id) // Удаление предмета по айди
        {
           var ItemToDelete = await _typeOfItemService.DeleteTypeByIdAsync(id);
            return ItemToDelete == null ? NoContent() : Ok("Предмет успешно удален!");
        }
        [HttpPut]
        public async Task<IActionResult> UpdateType(int id, string? UpdTypeName) //Изменение типа предмета по айди
        {
           var updItem = await _typeOfItemService.UpdateTypeAsync(id, UpdTypeName);
            return updItem == null ? NoContent() : Ok(updItem);
        }
    }
}
