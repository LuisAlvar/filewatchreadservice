using System;
using System.Collections.Generic;
using System.Text;

namespace FileWatchReadService.RabbitMQ
{
  public interface IAsyncDataServiceMessageQueue
  {
    bool EstablishConnection();
    bool PublishTaskToQueue(string Message);
  }
}
