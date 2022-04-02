using System;
using System.Collections.Generic;
using System.Text;

namespace PCPProviderFTPFileReadService.FileWatcher
{
  public interface IFileWatcherConfiguration
  {
    //public List<CustomFolderSettings> GetConfigurations();

    void StartFileSystemWatcher();
  }
}
