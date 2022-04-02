using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PCPProviderFTPFileReadService
{
  /// <summary>
  /// This class defines an individual type of file and its associated
  /// folder to be monitored by the File System Watcher
  /// </summary>
  public class CustomFolderSettings
  {
    /// <summary>Unique identifier of the combination File type/folder.
    /// Arbitrary number (for instance 001, 002, and so on)</summary>
    [XmlAttribute]
    public string FolderID { get; set; }
    /// <summary>If TRUE: the file type and folder will be monitored</summary>
    [XmlElement]
    public bool FolderEnabled { get; set; }
    /// <summary>Description of the type of files and folder location –
    /// Just for documentation purpose</summary>
    [XmlElement]
    public string FolderDescription { get; set; }
    /// <summary>Filter to select the type of files to be monitored.
    /// (Examples: *.shp, *.*, Project00*.zip)</summary>
    [XmlElement]
    public string FolderFilter { get; set; }
    /// <summary>Full path to be monitored
    /// (i.e.: D:\files\projects\shapes\ )</summary>
    [XmlElement]
    public string FolderPath { get; set; }
    /// <summary>If TRUE: the folder and its subfolders will be monitored</summary>
    [XmlElement]
    public bool FolderIncludeSub { get; set; }
    /// <summary>Specifies the command or action to be executed
    /// after an event has raised</summary>
    [XmlElement]
    public string ExecutableFile { get; set; }
    /// <summary>List of arguments to be passed to the executable file</summary>
    [XmlElement]
    public string ExecutableArguments { get; set; }
    /// <summary>Default constructor of the class</summary>       
    public CustomFolderSettings()
    {
    }
  }
}
