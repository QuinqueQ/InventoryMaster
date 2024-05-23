using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace BusinessLogicLayer.Services
{
    public class ZeroQuantityItemsCleanupService : BackgroundService // Сервис проверяющий нет ли в базе предеметов с количеством 0
    {
        private readonly ILogger<ZeroQuantityItemsCleanupService> _logger;  // Создал Логгер, который в консоли выводит информацию о работе этого сервиса
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ZeroQuantityItemsCleanupService(ILogger<ZeroQuantityItemsCleanupService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Служба очистки предметов с нулевым количеством работает.");

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ItemsDBContext>();

                    var zeroQuantityItems = await dbContext.Items.Where(item => item.Quantity <= 0).ToListAsync(cancellationToken: stoppingToken);
                    if (zeroQuantityItems.Any())
                    {
                        dbContext.Items.RemoveRange(zeroQuantityItems);
                        await dbContext.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation ($"Предметы с нулевым количеством были удалены из базы данных.");
                    }
                    else
                    {
                        _logger.LogInformation("Предметов с нулевым количеством в базе данных не найдено.");
                    }
                }
                // Период ожидания перед следующей проверкой 1 час 
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}