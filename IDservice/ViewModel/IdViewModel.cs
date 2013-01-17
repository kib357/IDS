using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows.Media.Imaging;
using IDservice.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace IDservice.ViewModel
{
    public partial class IdViewModel : NotificationObject
    {
        private AppModes _prevAppMode;
        private static string _startupPath { get { return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); }}

        private readonly string _configPath =
            Path.Combine(new[] { _startupPath, @"Layouts.xml" });

        public DelegateCommand<string> ChangeWindowStateCommand { get; set; }
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand AddItemCommand { get; set; }
        public DelegateCommand EditItemCommand { get; set; }
        public DelegateCommand DeleteItemCommand { get; set; }
        public DelegateCommand SaveItemCommand { get; set; }
        public DelegateCommand<object> SelectItemCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }

        public IdViewModel()
        {
            Initialize();
            ChangeWindowStateCommand = new DelegateCommand<string>(ChangeWindowState);
            BackCommand = new DelegateCommand(Back);
            AddItemCommand = new DelegateCommand(AddItem);
            EditItemCommand = new DelegateCommand(EditItem);
            DeleteItemCommand = new DelegateCommand(DeleteItem);
            SaveItemCommand = new DelegateCommand(SaveItem);
            SelectItemCommand = new DelegateCommand<object>(SelectItem);
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

        private void DeleteItem()
        {
            throw new NotImplementedException();
        }

        private void Back()
        {
            switch (AppMode)
            {
                case AppModes.ViewLayoutGroup:
                    AppMode = AppModes.LayoutGroups;
                    SelectedLayoutGroup = null;
                    SelectedLayout = null;
                    break;
                case AppModes.ViewLayout:
                    AppMode = AppModes.ViewLayoutGroup;
                    SelectedLayout = null;
                    break;
            }
        }

        private void SelectItem(object item)
        {
            switch (AppMode)
            {
                case AppModes.LayoutGroups:
                    SelectedLayoutGroup = (LayoutGroup)item;
                    AppMode = AppModes.ViewLayoutGroup;
                    break;
                case AppModes.ViewLayoutGroup:
                    SelectedLayout = (Layout)item;
                    AppMode = AppModes.ViewLayout;
                    break;
            }
        }

        private void EditItem()
        {
            switch (AppMode)
            {
                case AppModes.ViewLayoutGroup:
                    SelectedLayoutGroup.BeginEdit();
                    AppMode = AppModes.EditLayoutGroup;
                    break;
                case AppModes.ViewLayout:
                    SelectedLayout.BeginEdit();
                    AppMode = AppModes.EditLayout;
                    break;
            }
        }

        private void SaveItem()
        {
            switch (AppMode)
            {
                case AppModes.AddLayoutGroup:
                case AppModes.EditLayoutGroup:
                    SelectedLayoutGroup.EndEdit();
                    SaveLayoutGroup();
                    break;
                case AppModes.AddLayout:
                case AppModes.EditLayout:
                    SelectedLayout.EndEdit();
                    SaveLayout();
                    break;
            }
            SaveConfiguration();
        }

        private void SaveLayoutGroup()
        {

            var layoutGroup = LayoutGroups.FirstOrDefault(l => l.Id == SelectedLayoutGroup.Id);
            if (layoutGroup == null)
                LayoutGroups.Add(SelectedLayoutGroup);
            LayoutGroups = new ObservableCollection<LayoutGroup>(LayoutGroups);
            AppMode = AppModes.ViewLayoutGroup;
        }

        private void SaveLayout()
        {
            var layout = SelectedLayoutGroup.Layouts.FirstOrDefault(l => l.Id == SelectedLayout.Id);
            if (layout == null)
                SelectedLayoutGroup.Layouts.Add(SelectedLayout);
            Layouts = new ObservableCollection<Layout>(SelectedLayoutGroup.Layouts);
            AppMode = AppModes.ViewLayout;
        }

        private void Cancel()
        {
            if (SelectedLayoutGroup != null)
                SelectedLayoutGroup.CancelEdit();
            if (SelectedLayout != null)
                SelectedLayout.CancelEdit();
            AppMode = _prevAppMode;
        }

        private void AddItem()
        {
            switch (AppMode)
            {
                case AppModes.LayoutGroups:
                    SelectedLayoutGroup = new LayoutGroup();
                    AppMode = AppModes.AddLayoutGroup;
                    break;
                case AppModes.ViewLayoutGroup:
                    SelectedLayout = new Layout();
                    AppMode = AppModes.AddLayout;
                    break;
            }
        }

        public void LoadLayoutBackground(string fileName)
        {
            SelectedLayout.BackgroundImage = fileName;
            RaisePropertyChanged("SelectedLayout");
        }
    }
}
