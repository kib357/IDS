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

        private readonly string _configPath =
            Path.Combine(new[] { Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), @"Layouts.xml" });

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

        private void EditItem()
        {
            if (SelectedItem == null) return;
            SelectedItem.BeginEdit();
            if (SelectedItem is LayoutGroup)
                AppMode = AppModes.EditLayoutGroup;
        }

        private void Back()
        {
            switch (AppMode)
            {
                case AppModes.ViewLayoutGroup:
                    AppMode = AppModes.LayoutGroups;
                    break;
                case AppModes.ViewLayout:
                    AppMode = AppModes.ViewLayoutGroup;
                    break;
            }
        }

        private void SelectItem(object item)
        {
            if (item is IEditableItem)
            {
                SelectedItem = item as IEditableItem;
                if (SelectedItem is LayoutGroup)
                    AppMode = AppModes.ViewLayoutGroup;
            }
        }

        private void SaveItem()
        {
            if (SelectedItem == null)
                throw new Exception("Attemping to save not selected element");
            var layoutGroup = LayoutGroups.FirstOrDefault(l => l.Id == SelectedItem.Id);
            if (layoutGroup == null && SelectedItem is LayoutGroup)
                LayoutGroups.Add(SelectedItem as LayoutGroup);
            else
            {
                SelectedItem.EndEdit();
            }
            SaveConfiguration();
            LayoutGroups = new ObservableCollection<LayoutGroup>(LayoutGroups.OrderBy(lg => lg.Name));
            if (SelectedItem is LayoutGroup)
                AppMode = AppModes.ViewLayoutGroup;
        }

        private void Cancel()
        {
            if (SelectedItem != null)
                SelectedItem.CancelEdit();
            AppMode = _prevAppMode;
        }

        private void AddItem()
        {
            SelectedItem = new LayoutGroup();
            AppMode = AppModes.AddLayoutGroup;
        }               
    }
}
