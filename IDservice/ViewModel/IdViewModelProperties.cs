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

        //Print settings
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
                    var printcap = _selectedPrinter.GetPrintCapabilities();
                    CanPrintTwoSides = printcap.DuplexingCapability.Contains(Duplexing.TwoSidedLongEdge);
                    if (!CanPrintTwoSides) PrintTwoSides = false;
                    RaisePropertyChanged("CanPrintTwoSides");
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

        private bool _printBackground = true;
        public bool PrintBackground
        {
            get { return _printBackground; }
            set { _printBackground = value; RaisePropertyChanged("PrintBackground"); }
        }

        private bool _printTwoSides;
        public bool PrintTwoSides
        {
            get { return _printTwoSides; }
            set
            {
                _printTwoSides = value;
                var deltaTicket = new PrintTicket();
                deltaTicket.Duplexing = _printTwoSides ? Duplexing.TwoSidedLongEdge : Duplexing.OneSided;
                var result = SelectedPrinter.MergeAndValidatePrintTicket(SelectedPrinter.UserPrintTicket, deltaTicket);
                if (result.ValidatedPrintTicket.Duplexing == (_printTwoSides ? Duplexing.TwoSidedLongEdge : Duplexing.OneSided))
                {
                    SelectedPrinter.UserPrintTicket = result.ValidatedPrintTicket;
                    SelectedPrinter.Commit();
                }
                RaisePropertyChanged("PrintTwoSides");
            }
        }

        public bool CanPrintTwoSides { get; set; }

        private bool _printOtherside = true;
        public bool PrintOtherside
        {
            get { return _printOtherside; }
            set { _printOtherside = value; RaisePropertyChanged("PrintOtherside"); }
        }

        private double _printMarginX;
        public double PrintMarginX
        {
            get { return _printMarginX; }
            set { _printMarginX = value; RaisePropertyChanged("PrintMarginX"); RaisePropertyChanged("PrintMargin"); }
        }

        private double _printMarginY;
        public double PrintMarginY
        {
            get { return _printMarginY; }
            set { _printMarginY = value; RaisePropertyChanged("PrintMarginY"); RaisePropertyChanged("PrintMargin"); }
        }

        public Thickness PrintMargin
        {
            get {return new Thickness(PrintMarginX, PrintMarginY,0,0);}
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

        private string _otherside;
        public string Otherside
        {
            get { return _otherside; }
            set { _otherside = value; RaisePropertyChanged("Otherside"); }
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
                LoadLayoutOtherside();
                RaisePropertyChanged("SelectedLayout");
                RaisePropertyChanged("Background");
                RaisePropertyChanged("Otherside");
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
