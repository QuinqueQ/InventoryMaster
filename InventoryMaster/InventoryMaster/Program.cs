using InventoryMaster.Interfaces;
using InventoryMaster.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Text.Json.Serialization;

namespace InventoryMaster
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //пытался сделать логгер
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}") // Вывод в консоль
               .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}") // Вывод в файл с тем же форматом
               .CreateLogger();

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

            builder.Services.AddHostedService<ZeroQuantityItemsCleanupService>();//o

            Log.Information("Запись лога в текстовый файл");

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