using FileWatchReadService.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PCPProviderFTPFileReadService.FileWatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileWatchReadService
{
  public class Worker : BackgroundService
  {
    private readonly ILogger<Worker> _logger;
    private readonly FileWatcherConfiguration _configuration;
    private readonly AsyncDataServiceMessageQueue _publish;

    public Worker(ILogger<Worker> logger, FileWatcherConfiguration configuration, AsyncDataServiceMessageQueue publish)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _publish = publish ?? throw new ArgumentNullException(nameof(publish));
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

      stoppingToken.ThrowIfCancellationRequested();

      _configuration.StartFileSystemWatcher();

      return Task.CompletedTask;
    }
  }
}
