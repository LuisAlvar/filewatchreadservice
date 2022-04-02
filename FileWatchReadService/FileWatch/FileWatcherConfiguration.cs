using FileWatchReadService.RabbitMQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace PCPProviderFTPFileReadService.FileWatcher
{
  public class FileWatcherConfiguration : IFileWatcherConfiguration
  {
    private readonly IConfiguration _configuration;
    private readonly AsyncDataServiceMessageQueue _publish;
    public readonly ILogger<FileWatcherConfiguration> _logger;

    private string FileNameXML { get; set; }
    private List<CustomFolderSettings>? lstFolders { get; set; }

    public FileWatcherConfiguration(ILogger<FileWatcherConfiguration> logger, IConfiguration configuration, AsyncDataServiceMessageQueue publish)
    {
      _configuration = configuration;
      _publish = publish;
      _logger = logger;

      GetConfigurations();
    }

    private void GetConfigurations()
    {
      if (string.IsNullOrEmpty(FileNameXML))
      {
        FileNameXML = ConfigurationManager.AppSettings["XMLFileFolderSettings"].ToString();

        EventLog.WriteEntry("FileWatchReadService", "---> The name of the File Watcher Custom Settings xml: " + FileNameXML, EventLogEntryType.Information);

        XmlSerializer deserializer = new XmlSerializer(typeof(List<CustomFolderSettings>));
        TextReader reader = new StreamReader(FileNameXML);
        object obj = deserializer.Deserialize(reader);

        reader.Close();
        
        lstFolders = obj as List<CustomFolderSettings>;
      }
    }

    public void StartFileSystemWatcher()
    {
      if (lstFolders != null)
      {
        List<FileSystemWatcher> listFileSystemWatcher = new List<FileSystemWatcher>();

        foreach (CustomFolderSettings customFolder in lstFolders)
        {
          DirectoryInfo dir = new DirectoryInfo(customFolder.FolderPath);

          // Checks whether the folder is enabled and
          // also the directory is a valid location
          if (customFolder.FolderEnabled && dir.Exists)
          {
            // Creates a new instance of FileSystemWatcher
            FileSystemWatcher fileSWatch = new FileSystemWatcher();

            // Sets the filter
            fileSWatch.Filter = customFolder.FolderFilter;

            // Sets the folder location
            fileSWatch.Path = customFolder.FolderPath;

            // Sets the action to be executed
            StringBuilder actionToExecute = new StringBuilder(customFolder.ExecutableFile);

            // List of arguments
            StringBuilder actionArguments = new StringBuilder(customFolder.ExecutableArguments);

            // Subscribe to notify filters
            fileSWatch.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName |
              NotifyFilters.DirectoryName;

            // Associate the event that will be triggered when a new file
            // is added to the monitored folder, using a lambda expression                   
            fileSWatch.Created += (senderObj, fileSysArgs) =>
              fileSWatch_Created(senderObj, fileSysArgs,
               actionToExecute.ToString(), actionArguments.ToString());

            // Begin watching
            fileSWatch.EnableRaisingEvents = true;

            // Add the systemWatcher to the list
            listFileSystemWatcher.Add(fileSWatch);

            // Record a log entry into Windows Event Log
            EventLog.WriteEntry("FileWatchReadService", String.Format(
              "Starting to monitor files with extension ({0}) in the folder ({1})",
              fileSWatch.Filter, fileSWatch.Path).ToString(), EventLogEntryType.Information);


          }
        }
      }
    }

    void fileSWatch_Created(object sender, FileSystemEventArgs e, string action_Exec, string action_Args)
    {
      string fileName = e.FullPath;
      string newStr = string.Format(action_Args, fileName);

      EventLog.WriteEntry("FileWatchReadService", $"---> File ({fileName}) )", EventLogEntryType.Information);
      System.Console.WriteLine($"---> File ({fileName}) )");

      ProcessMessage(fileName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="FilePathName"></param>
    private void ProcessMessage(string FilePathName)
    {
      try
      {
        if (!_publish.EstablishConnection())
        {
          string Message = $"Unable to publish: {FilePathName}, Unable to connection to RabbitMQ please check if serivce is running properly";
          EventLog.WriteEntry("FileWatchReadService", Message, EventLogEntryType.Error);
          System.Console.WriteLine(Message);
          return;
        }

        _publish.PublishTaskToQueue(FilePathName);
      }
      catch (Exception ex)
      {

        throw ex;
      }
    }

  }

}
