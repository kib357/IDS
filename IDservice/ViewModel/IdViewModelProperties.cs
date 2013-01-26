using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using IDservice.Model;

namespace IDservice.ViewModel
{
    public partial class IdViewModel
    {
        private AppModes _appMode;
        public AppModes AppMode
        {
            get { return _appMode; }
            set
            {
                ShowDeleteConfirmation = false;
                _prevAppMode = _appMode;
                _appMode = value;
                switch (_appMode)
                {
                    case AppModes.LayoutGroups:
                        Title = "Мероприятия";
                        break;
                    case AppModes.ViewLayoutGroup:
                        Title = SelectedLayoutGroup.Name;
                        break;
                    case AppModes.AddLayoutGroup:
                        Title = "Добавление мероприятия";
                        break;
                    case AppModes.EditLayoutGroup:
                        Title = "Изменение мероприятия";
                        break;
                    case AppModes.ViewLayout:
                        Title = SelectedLayout.Name;
                        break;
                    case AppModes.AddLayout:
                        Title = "Добавление макета";
                        break;
                    case AppModes.EditLayout:
                        Title = "Изменение макета";
                        break;
                    case AppModes.PrintCards:
                        Title = "Печать бейджей";
                        break;
                }
                RaisePropertyChanged("AppMode");
            }
        }

        private PrintQueue _selectedPrinter;
        public PrintQueue SelectedPrinter
        {
            get { return _selectedPrinter; }
            set
            {
                _selectedPrinter = value;
                if (_selectedPrinter != null && _selectedPrinter.UserPrintTicket != null)
                {
                    if (_selectedPrinter.UserPrintTicket.PageMediaSize.Width != null)
                        PageWidth = (double) _selectedPrinter.UserPrintTicket.PageMediaSize.Width;
                    if (_selectedPrinter.UserPrintTicket.PageMediaSize.Height != null)
                        PageHeight = (double) _selectedPrinter.UserPrintTicket.PageMediaSize.Height;
                    RaisePropertyChanged("PageWidth");
                    RaisePropertyChanged("PageHeight");
                }
                RaisePropertyChanged("SelectedPrinter");                
            }
        }

        public double PageWidth { get; set; }
        public double PageHeight { get; set; }

        private PrintQueueCollection _printers;
        public PrintQueueCollection Printers
        {
            get { return _printers; }
            set { _printers = value; RaisePropertyChanged("Printers"); }
        }

        private string _cardUserName = "Фамилия Имя";
        public string CardUserName
        {
            get { return _cardUserName; }
            set { _cardUserName = value; RaisePropertyChanged("CardUserName"); }
        }

        private FileSystemWatcher _watcher;
        private string _photoPath;
        public string PhotoPath
        {
            get { return _photoPath; }
            set
            {
                _photoPath = value;
                LoadImages();
                _watcher = new FileSystemWatcher(_photoPath);
                _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
                // Add event handlers.
                _watcher.Created += OnChanged;
                _watcher.Deleted += OnChanged;
                // Begin watching.
                _watcher.EnableRaisingEvents = true;
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(LoadImages));
        }

        private void LoadImages()
        {
            if (!Directory.Exists(_photoPath)) return;
            ImageList.Clear();
            var allImageFiles = Directory.GetFiles(_photoPath).
                                          Select(s => new FileInfo(s)).
                                          Where(f => f.Extension.ToLower() == ".jpeg" || f.Extension.ToLower() == ".jpg").
                                          OrderByDescending(x => x.LastWriteTime);
            foreach (var file in allImageFiles)
                ImageList.Add(file.ToString());
        }

        private ObservableCollection<string> _imageList = new ObservableCollection<string>();
        public ObservableCollection<string> ImageList
        {
            get { return _imageList; }
            set { _imageList = value; RaisePropertyChanged("ImageList"); }
        }

        private string _photo;
        public string Photo
        {
            get { return _photo; }
            set { _photo = value; RaisePropertyChanged("Photo"); }
        }

        private string _background;
        public string Background
        {
            get { return _background; }
            set { _background = value; RaisePropertyChanged("Background"); }
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

        private LayoutGroup _selectedLayoutGroup;
        public LayoutGroup SelectedLayoutGroup
        {
            get { return _selectedLayoutGroup; }
            set
            {
                _selectedLayoutGroup = value;
                if (_selectedLayoutGroup != null)
                    Layouts = _selectedLayoutGroup.Layouts;
                RaisePropertyChanged("SelectedLayoutGroup");
            }
        }

        private ObservableCollection<Layout> _layouts = new ObservableCollection<Layout>();
        public ObservableCollection<Layout> Layouts
        {
            get { return _layouts; }
            set { _layouts = value; RaisePropertyChanged("Layouts"); }
        }

        private Layout _selectedLayout;
        public Layout SelectedLayout
        {
            get { return _selectedLayout; }
            set
            {
                _selectedLayout = value;
                LoadLayoutBackground();
                RaisePropertyChanged("SelectedLayout");
                RaisePropertyChanged("Background");
            }
        }

        private bool _showDeleteConfirmation;
        public bool ShowDeleteConfirmation
        {
            get { return _showDeleteConfirmation; }
            set { _showDeleteConfirmation = value; RaisePropertyChanged("ShowDeleteConfirmation"); }
        }
    }
}
