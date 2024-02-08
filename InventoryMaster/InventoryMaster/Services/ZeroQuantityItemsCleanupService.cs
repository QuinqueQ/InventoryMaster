using Microsoft.EntityFrameworkCore;
using InventoryMaster;

namespace InventoryMaster.Services
{
    public class ZeroQuantityItemsCleanupService : BackgroundService // сервис проверяющий нет ли в базе предеметов с количеством 0
    {
        private readonly ILogger<ZeroQuantityItemsCleanupService> _logger;
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

                    var zeroQuantityItems = await dbContext.Items.Where(item => item.Quantity == 0).ToListAsync(cancellationToken: stoppingToken);
                    if (zeroQuantityItems.Any())
                    {
                        dbContext.Items.RemoveRange(zeroQuantityItems);
                        await dbContext.SaveChangesAsync(stoppingToken);
                        string message = $"{zeroQuantityItems.Count} Предметы с нулевым количеством были удалены из базы данных.";
                        _logger.LogInformation(message);
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