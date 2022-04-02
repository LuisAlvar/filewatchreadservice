using FileWatchReadService.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PCPProviderFTPFileReadService.FileWatcher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FileWatchReadService
{
  public class Program
  {
    public static void Main(string[] args)
    {

      string LogName = "FileWatchReadService";
      string SourceName = "FileWatchReadService";

      if (!EventLog.SourceExists(SourceName))
      {
        EventSourceCreationData newSourceData = new EventSourceCreationData(SourceName, LogName);
        EventLog.CreateEventSource(newSourceData);

        using (EventLog _log = new EventLog(LogName, Environment.MachineName, SourceName))
        {
          OverflowAction overflowAction = OverflowAction.OverwriteAsNeeded;
          Int32 numDays = _log.MinimumRetentionDays;

          _log.ModifyOverflowPolicy(overflowAction, numDays);
        }
      }

      CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
              services.AddHostedService<Worker>();
              services.AddSingleton<FileWatcherConfiguration>();
              services.AddSingleton<AsyncDataServiceMessageQueue>();
            });
  }
}
