using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using IDservice.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace IDservice.ViewModel
{
    public partial class IdViewModel : NotificationObject
    {
        private AppModes _prevAppMode;
        private static string _startupPath { get { return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); } }

        private readonly string _configPath;
        private readonly string _imagesPath;        

        public DelegateCommand<string> ChangeWindowStateCommand { get; set; }
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand AddItemCommand { get; set; }
        public DelegateCommand EditItemCommand { get; set; }
        public DelegateCommand DeleteItemCommand { get; set; }
        public DelegateCommand DeletePreviewCommand { get; set; }
        public DelegateCommand SaveItemCommand { get; set; }
        public DelegateCommand<object> SelectItemCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        public DelegateCommand PrintCardsCommand { get; set; }

        public IdViewModel()
        {
            _configPath = Path.Combine(new[] { _startupPath, @"Layouts.xml" });
            _imagesPath = Path.Combine(new[] { _startupPath, @"images" });            
            Initialize();
            ChangeWindowStateCommand = new DelegateCommand<string>(ChangeWindowState);
            BackCommand = new DelegateCommand(Back);
            AddItemCommand = new DelegateCommand(AddItem);
            EditItemCommand = new DelegateCommand(EditItem);
            DeleteItemCommand = new DelegateCommand(DeleteItem);
            DeletePreviewCommand = new DelegateCommand(DeletePreview);
            SaveItemCommand = new DelegateCommand(SaveItem);
            SelectItemCommand = new DelegateCommand<object>(SelectItem);
            CancelCommand = new DelegateCommand(Cancel);
            PrintCardsCommand = new DelegateCommand(PrintCards);
            AppMode = AppModes.LayoutGroups;

            var printServer = new LocalPrintServer();            
            Printers = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            var sPrinter = LocalPrintServer.GetDefaultPrintQueue();
            SelectedPrinter = Printers.FirstOrDefault(p => p.Name == sPrinter.Name && p.FullName == sPrinter.FullName);

            PhotoPath = Path.Combine(_startupPath, "photo");
        }

        private void PrintCards()
        {
            AppMode = AppModes.PrintCards;
            if (Layouts != null)
                SelectedLayout = Layouts.FirstOrDefault();
        }

        private void DeletePreview()
        {
            ShowDeleteConfirmation = true;
        }

        private void DeleteItem()
        {
            switch (AppMode)
            {
                case AppModes.ViewLayoutGroup:
                    LayoutGroups.Remove(SelectedLayoutGroup);
                    AppMode = AppModes.LayoutGroups;
                    SelectedLayoutGroup = null;
                    SelectedLayout = null;
                    break;
                case AppModes.ViewLayout:
                    Task.Factory.StartNew(() => TryDeleteLayoutImages(), TaskCreationOptions.LongRunning);
                    SelectedLayoutGroup.Layouts.Remove(SelectedLayout);
                    Layouts = SelectedLayoutGroup.Layouts;
                    AppMode = AppModes.ViewLayoutGroup;
                    SelectedLayout = null;
                    break;
            }
            SaveConfiguration();
        }

        private bool TryDeleteLayoutImages()
        {            
            try
            {
                var backgroundPath = Path.Combine(_imagesPath, SelectedLayout.Id + "_background.jpg");
                var othersidePath = Path.Combine(_imagesPath, SelectedLayout.Id + "_otherside.jpg");
                if (File.Exists(backgroundPath))
                    File.Delete(backgroundPath);
                if (File.Exists(othersidePath))
                    File.Delete(othersidePath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }                    
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
                case AppModes.PrintCards:
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
                case AppModes.PrintCards:
                    SelectedLayout = (Layout)item;
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

        public void LoadNewLayoutBackground(string fileName)
        {
            Background = null;

            var objImage = new BitmapImage();
            objImage.BeginInit();
            objImage.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);
            objImage.CacheOption = BitmapCacheOption.OnLoad;
            objImage.EndInit();

            var encoder = new JpegBitmapEncoder();
            var photolocation = Path.Combine(_imagesPath, SelectedLayout.Id + "_background.jpg");  //file name 
            encoder.Frames.Add(BitmapFrame.Create(objImage));
            using (var filestream = new FileStream(photolocation, FileMode.Create))
                encoder.Save(filestream);

            LoadLayoutBackground();
            RaisePropertyChanged("SelectedLayout");
        }

        public void LoadNewLayoutOtherside(string fileName)
        {
            Otherside = null;

            var objImage = new BitmapImage();
            objImage.BeginInit();
            objImage.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);
            objImage.CacheOption = BitmapCacheOption.OnLoad;
            objImage.EndInit();

            var encoder = new JpegBitmapEncoder();
            var photolocation = Path.Combine(_imagesPath, SelectedLayout.Id + "_otherside.jpg");  //file name 
            encoder.Frames.Add(BitmapFrame.Create(objImage));
            using (var filestream = new FileStream(photolocation, FileMode.Create))
                encoder.Save(filestream);

            LoadLayoutOtherside();
            RaisePropertyChanged("SelectedLayout");
        }

        private void LoadLayoutBackground()
        {
            if (SelectedLayout == null) return;
            var path = Path.Combine(_imagesPath, SelectedLayout.Id + "_background.jpg");
            Background = path;
        }

        private void LoadLayoutOtherside()
        {
            if (SelectedLayout == null) return;
            var path = Path.Combine(_imagesPath, SelectedLayout.Id + "_otherside.jpg");
            Otherside = path;
        }
    }
}
