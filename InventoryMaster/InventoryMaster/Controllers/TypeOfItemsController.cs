using InventoryMaster.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMaster.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TypeOfItemsController : ControllerBase
    {
        private readonly ITypeOfItemService _typeOfItemService; // сервис для работы с предметами типов из базы данных

        public TypeOfItemsController(ITypeOfItemService typeOfItemService)
        {
            _typeOfItemService = typeOfItemService;
        }

        [HttpGet(Name = "TypeGet")]
        public async Task<IActionResult> Get()  // Получаем все типы предметов
        {
           var typeOfItems = await _typeOfItemService.GetTypeOfItemsAsync();
            return typeOfItems == null ? NoContent() : Ok(typeOfItems);
        }


        [HttpPost(Name = "TypePost")]
        public async Task<IActionResult> PostType(string? TypeName) // Создание нового типа предмета
        {
            var typeOfItems = await _typeOfItemService.PostTypeAsync(TypeName);
            return typeOfItems == null ? NoContent() : Ok(typeOfItems);
        }

        [HttpDelete(Name = "TypeDelete")]
        public async Task<IActionResult> DeleteTypeById(int typeId) // Удаление предмета по айди
        {
           var ItemToDelete = await _typeOfItemService.DeleteTypeByIdAsync(typeId);
            return ItemToDelete == null ? NoContent() : Ok("Предмет успешно удален!");
        }
        [HttpPut(Name ="TypeUpdate")]
        public async Task<IActionResult> UpdateType(int id, string? UpdTypeName) //Изменение типа предмета по айди
        {
           var updItem = await _typeOfItemService.UpdateTypeAsync(id, UpdTypeName);
            return updItem == null ? NoContent() : Ok(updItem);
        }
    }
}
