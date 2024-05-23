using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Services;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DataAccessLayer.Data;

namespace PresentationLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ����������� ��������� ���� ������
            builder.Services.AddDbContext<ItemsDBContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ����������� �������
            builder.Services.AddScoped<IItemService, ItemService>();
            builder.Services.AddScoped<ITypeOfItemService, TypeOfItemService>();

            // ���������� ������������
            builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // ���������� ��������� OpenAPI ������������
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHostedService<ZeroQuantityItemsCleanupService>();

            Log.Information("������ ���� � ��������� ����");

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}