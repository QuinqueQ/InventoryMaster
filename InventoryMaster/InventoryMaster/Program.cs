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

            // Пытался выводить логи в отдельный файл
            Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}")
            .CreateLogger();


            // Регистрация контекста базы данных
            builder.Services.AddDbContext<ItemsDBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Регистрация сервиса
            builder.Services.AddTransient<IItemService, ItemService>();

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

<<<<<<< HEAD
            //JanarGay
=======
            // Разрешение маршрутов для контроллеров
>>>>>>> Nazarq
            app.MapControllers();

            app.Run();
        }
    }
}