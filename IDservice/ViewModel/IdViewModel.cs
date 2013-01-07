using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using IDservice.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace IDservice.ViewModel
{
    public enum AppModes
    {
        LayoutGroups,
        AddLayoutGroup,
        ViewLayoutGroup,
        EditLayoutGroup,
        AddLayout,
        ViewLayout,
        EditLayout,
        PrintCards
    }

    public class IdViewModel : NotificationObject
    {
        private AppModes _prevAppMode;
        private AppModes _appMode;
        public AppModes AppMode
        {
            get { return _appMode; }
            set
            {
                _prevAppMode = _appMode;
                _appMode = value;                
                switch (_appMode)
                {
                    case AppModes.LayoutGroups:
                        Title = "Мероприятия";
                        break;
                    case AppModes.AddLayoutGroup:
                        Title = "Добавление мероприятия";
                        break;
                    case AppModes.ViewLayoutGroup:
                        Title = SelectedLayoutGroup.Name;
                        break;
                    case AppModes.EditLayoutGroup:
                        Title = "Изменение мероприятия";
                        break;
                }
                RaisePropertyChanged("AppMode");
            }
        }

        private LayoutGroup _selectedLayoutGroup;
        public LayoutGroup SelectedLayoutGroup
        {
            get { return _selectedLayoutGroup; }
            set { _selectedLayoutGroup = value; RaisePropertyChanged("SelectedLayoutGroup"); }
        }

        private readonly string _configPath =
            Path.Combine(new[] { Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), @"Layouts.xml" });

        public DelegateCommand<string> ChangeWindowStateCommand { get; set; }
        public DelegateCommand AddItemCommand { get; set; }
        public DelegateCommand SaveItemCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public IdViewModel()
        {
            Initialize();
            ChangeWindowStateCommand = new DelegateCommand<string>(ChangeWindowState);
            AddItemCommand = new DelegateCommand(AddItem);
            SaveItemCommand = new DelegateCommand(SaveItem);
            CancelCommand = new DelegateCommand(Cancel);
            AppMode = AppModes.LayoutGroups;

            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri("background.jpg", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            Background = src;

            var printServer = new LocalPrintServer();

            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });

            foreach (PrintQueue printer in printQueuesOnLocalServer)
            {
                Debug.WriteLine("\tThe shared printer : " + printer.Name);
            }
        }

        private void SaveItem()
        {
            var layoutGroup = LayoutGroups.FirstOrDefault(l => l.Id == SelectedLayoutGroup.Id);
            if (layoutGroup == null)
                LayoutGroups.Add(SelectedLayoutGroup);
            else
            {
                SelectedLayoutGroup.PartialClone(layoutGroup);
            }
            SelectedLayoutGroup = null;
            SaveConfiguration();
            AppMode = _prevAppMode;
        }

        private void Cancel()
        {
            AppMode = _prevAppMode;
        }

        private void AddItem()
        {
            SelectedLayoutGroup = new LayoutGroup();
            AppMode = AppModes.AddLayoutGroup;
        }

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
                using (var stream = File.OpenWrite(_configPath))
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

        private BitmapImage _background;

        public BitmapImage Background
        {
            get { return _background; }
            set
            {
                _background = value;
                RaisePropertyChanged("Background");
            }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; RaisePropertyChanged("Title"); }
        }

        private ObservableCollection<LayoutGroup> _layoutGroups = new ObservableCollection<LayoutGroup>();
        public ObservableCollection<LayoutGroup> LayoutGroups
        {
            get { return _layoutGroups; }
            set { _layoutGroups = value; RaisePropertyChanged("LayoutGroups"); }
        }
    }
}
