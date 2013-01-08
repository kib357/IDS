using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using IDservice.Model;

namespace IDservice.ViewModel
{
    public partial class IdViewModel
    {
        private void Initialize()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<LayoutGroup>));
                using (var stream = File.OpenRead(_configPath))
                {
                    var reader = new XmlTextReader(stream);
                    if (serializer.CanDeserialize(reader))
                        LayoutGroups = (ObservableCollection<LayoutGroup>)serializer.Deserialize(reader);
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
                if (!File.Exists(_configPath))
                {
                    var stream = File.Create(_configPath);
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

        private void SaveConfiguration()
        {
            try
            {
                var serializer = new XmlSerializer(typeof(ObservableCollection<LayoutGroup>));
                using (var stream = File.Create(_configPath))
                {
                    serializer.Serialize(stream, LayoutGroups);
                }
            }
            catch (Exception ex)
            {
                throw new Exception();
                //todo: show exception to user and close application
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
