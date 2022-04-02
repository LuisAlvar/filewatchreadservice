using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FileWatchReadService.RabbitMQ
{
  public class AsyncDataServiceMessageQueue: IAsyncDataServiceMessageQueue
  {
    private readonly ILogger<AsyncDataServiceMessageQueue> _logger;
    private ConnectionFactory? _factory;
    private IConnection? _connection;

    public AsyncDataServiceMessageQueue(ILogger<AsyncDataServiceMessageQueue> logger)
    {
      _logger = logger;
    }

    public bool EstablishConnection()
    {
      try
      {
        _factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = _factory.CreateConnection();
        return true;
      }
      catch (Exception ex)
      {
        string strLayoutErrorMessage = "Error Occured at EstablishConnection: "
          + "\nMessage - " + ex.Message
          + "\nSource - " + ex.Source
          + "\nStack Trace - " +  ex.StackTrace;

        _logger.LogError(strLayoutErrorMessage);
        return false;
      }
    }

    public bool PublishTaskToQueue(string Message)
    {
      try
      {
        byte[] MessageInBytes = Encoding.UTF8.GetBytes(Message);

        if (_connection != null)
        {
          var channel = _connection.CreateModel();

          channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

          var properties = channel.CreateBasicProperties();
          properties.Persistent = true;

          channel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: properties, body: MessageInBytes);

          _logger.LogWarning($"[x] sent {Message}");

          return true;
        }
        else
        {
          _logger.LogWarning("Please make sure to establish connection before calling on this method to publish a task message.");
        }

      }
      catch (Exception ex)
      {
        string strLayoutErrorMessage = "Error Occured at EstablishConnection: "
          + "\nMessage - " + ex.Message
          + "\nSource - " + ex.Source
          + "\nStack Trace - " + ex.StackTrace;

        _logger.LogError(strLayoutErrorMessage);
      }

      return false;
    }
  }
}
