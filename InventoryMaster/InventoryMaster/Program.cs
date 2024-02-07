using InventoryMaster.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

namespace InventoryMaster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Регистрация контекста базы данных
            builder.Services.AddDbContext<ItemsDBContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Регистрация сервиса
            builder.Services.AddScoped<IItemService, ItemService>();

            // Добавление контроллеров
            builder.Services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // Добавление генерации OpenAPI спецификации
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                // Включение Swagger UI в режиме разработки
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            // Разрешение маршрутов для контроллеров
            app.MapControllers();

            app.Run();
        }
    }
}
