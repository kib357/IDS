using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using IDservice.Model;
using Nini.Config;

namespace IDservice.ViewModel
{
    public partial class IdViewModel
    {
        private void Initialize()
        {
            CheckImageFolderExists();
            LoadConfig();
            LoadLayouts();
        }

        private void LoadConfig()
        {
            try
            {
                _configSource = new XmlConfigSource(Path.Combine(StartupPath, "Config.xml")) { AutoSave = true };
            }
            catch (Exception ex)
            {
                WriteDefaultConfig();
            }
            InitializeFields();
        }

        private void InitializeFields()
        {
            if (!_configSource.Configs["Main"].Contains("CardUserPhotoPath"))
                _configSource.Configs["Main"].Set("CardUserPhotoPath", CardUserPhotoPath ?? "");
            CardUserPhotoPath = _configSource.Configs["Main"].Get("CardUserPhotoPath");
        }

        private void SetConfigProperty(string propertyName, object value, string section = "Main")
        {
            _configSource.Configs[section].Set(propertyName, value);
        }

        private void WriteDefaultConfig()
        {
            _configSource = new XmlConfigSource();
            _configSource.AddConfig("Main");
            try
            {
                _configSource.Save(Path.Combine(StartupPath, "Config.xml"));
                _configSource.AutoSave = true;
            }
            catch { }
        }

        private void CheckImageFolderExists()
        {
            if (!Directory.Exists(ImagesPath))
                Directory.CreateDirectory(ImagesPath);
        }

        private void LoadLayouts()
        {
            try
            {
                var serializer = new XmlSerializer(typeof (ObservableCollection<LayoutGroup>));
                using (var stream = File.OpenRead(ConfigPath))
                {
                    var reader = new XmlTextReader(stream);
                    if (serializer.CanDeserialize(reader))
                        LayoutGroups = (ObservableCollection<LayoutGroup>) serializer.Deserialize(reader);
                    else
                    {
                        throw new Exception();
                        //todo: show exception to user and close application
                    }
                }
            }
            catch (Exception ex)
            {
                WriteInitialConfiguration();
            }
        }

        private void WriteInitialConfiguration()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    var backupPath = Path.Combine(StartupPath, "Layouts_backup.xml");
                    File.Delete(backupPath);
                    File.Move(ConfigPath, backupPath);
                }
                if (!File.Exists(ConfigPath))
                {
                    var stream = File.Create(ConfigPath);
                    stream.Close();
                }
            }
            catch (Exception)
            {
                throw new Exception();
                //todo: show exception to user and close application
            }
            SaveConfiguration();
        }

        private void SaveConfiguration(object state = null)
        {
            lock (SyncRoot)
            {
                try
                {
                    var serializer = new XmlSerializer(typeof (ObservableCollection<LayoutGroup>));
                    using (var stream = File.Create(ConfigPath))
                    {
                        serializer.Serialize(stream, LayoutGroups);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Save error");
                    //todo: show exception to user and close application
                }
            }
        }

        private void ChangeWindowState(string param)
        {
            if (Application.Current != null && Application.Current.MainWindow != null)
                switch (param)
                {
                    case "minimize":
                        Application.Current.MainWindow.WindowState = WindowState.Minimized;
                        break;
                    case "close":
                        Application.Current.MainWindow.Close();
                        break;
                }
        }
    }
}
