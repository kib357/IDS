using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using IDservice.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Nini.Config;

namespace IDservice.ViewModel
{
    public partial class IdViewModel : NotificationObject
    {
        private static object SyncRoot = new object();
        private XmlConfigSource _configSource;
        private AppModes _prevAppMode;
        private FileSystemWatcher _watcher;
        public static string StartupPath { get { return Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); } }
        public static readonly string ImagesPath = Path.Combine(new[] { StartupPath, @"images" });
        private static readonly string ConfigPath = Path.Combine(new[] { StartupPath, @"Layouts.xml" });                      

        public IdViewModel()
        {
            //_configPath = Path.Combine(new[] { StartupPath, @"Layouts.xml" });
            //_imagesPath
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

            //var printServer = new LocalPrintServer();            
            //Printers = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });
            //var sPrinter = LocalPrintServer.GetDefaultPrintQueue();
            //SelectedPrinter = Printers.FirstOrDefault(p => p.Name == sPrinter.Name && p.FullName == sPrinter.FullName);

            //CardUserPhotoPath = Path.Combine(StartupPath, "photo");

            Application.Current.Exit += OnExit;
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            SaveConfiguration();
        }

        #region Device settings

        public double PageWidth { get; set; }
        public double PageHeight { get; set; }   

        private PrintQueueCollection _printers;
        public PrintQueueCollection Printers
        {
            get { return _printers; }
            set { _printers = value; RaisePropertyChanged("Printers"); }
        }

        private PrintQueue _selectedPrinter;
        public PrintQueue SelectedPrinter
        {
            get { return _selectedPrinter; }
            set
            {
                _selectedPrinter = value;
                //TrySetPageSize();
                //if (_selectedPrinter != null && _selectedPrinter.UserPrintTicket != null)
                //{
                //    if (_selectedPrinter.UserPrintTicket.PageMediaSize.Width != null)
                //        PageWidth = (double)_selectedPrinter.UserPrintTicket.PageMediaSize.Width;
                //    if (_selectedPrinter.UserPrintTicket.PageMediaSize.Height != null)
                //        PageHeight = (double)_selectedPrinter.UserPrintTicket.PageMediaSize.Height;
                //    var printcap = _selectedPrinter.GetPrintCapabilities();
                //    CanPrintTwoSides = printcap.DuplexingCapability.Contains(Duplexing.TwoSidedLongEdge);
                //    if (!CanPrintTwoSides) PrintTwoSides = false;
                //    RaisePropertyChanged("CanPrintTwoSides");
                //    RaisePropertyChanged("PageWidth");
                //    RaisePropertyChanged("PageHeight");
                //}
                RaisePropertyChanged("SelectedPrinter");
            }
        }

        public bool CanPrintTwoSides { get; set; }

        private bool _printTwoSides;
        public bool PrintTwoSides
        {
            get { return _printTwoSides; }
            set
            {
                _printTwoSides = value;
                //var deltaTicket = new PrintTicket();
                //deltaTicket.Duplexing = _printTwoSides ? Duplexing.TwoSidedLongEdge : Duplexing.OneSided;
                //var result = SelectedPrinter.MergeAndValidatePrintTicket(SelectedPrinter.UserPrintTicket, deltaTicket);
                //if (result.ValidatedPrintTicket.Duplexing == (_printTwoSides ? Duplexing.TwoSidedLongEdge : Duplexing.OneSided))
                //{
                //    SelectedPrinter.UserPrintTicket = result.ValidatedPrintTicket;
                //    SelectedPrinter.Commit();
                //}
                RaisePropertyChanged("PrintTwoSides");
            }
        }

        private void TrySetPageSize()
        {
            //if (SelectedLayout == null) return;
            //var size = new PageMediaSize(PageMediaSizeName.Unknown, SelectedLayout.Width, SelectedLayout.Height);
            //var deltaTicket = new PrintTicket();
            //deltaTicket.PageMediaSize = size;
            //SelectedPrinter.UserPrintTicket.PageMediaSize = size;
            //SelectedPrinter.UserPrintTicket.PageBorderless = PageBorderless.Borderless;
            //SelectedPrinter.Commit();
            //var result = SelectedPrinter.MergeAndValidatePrintTicket(SelectedPrinter.UserPrintTicket, deltaTicket);
            //if (result.ValidatedPrintTicket.PageMediaSize == size)
            //{
            //    SelectedPrinter.UserPrintTicket = result.ValidatedPrintTicket;
            //    SelectedPrinter.Commit();
            //}
        }

        #endregion

        #region CardUser

        private string _cardUserPhoto;
        public string CardUserPhoto
        {
            get { return _cardUserPhoto; }
            set { _cardUserPhoto = value; RaisePropertyChanged("CardUserPhoto"); }
        }

        private ObservableCollection<string> _cardUserPhotoList = new ObservableCollection<string>();
        public ObservableCollection<string> CardUserPhotoList
        {
            get { return _cardUserPhotoList; }
            set { _cardUserPhotoList = value; RaisePropertyChanged("CardUserPhotoList"); }
        }

        private string _cardUserPhotoPath;
        public string CardUserPhotoPath
        {
            get { return _cardUserPhotoPath; }
            set
            {
                _cardUserPhotoPath = value;
                SetConfigProperty("CardUserPhotoPath", _cardUserPhotoPath);
                if (!Directory.Exists(_cardUserPhotoPath)) return;
                LoadCardUserPhotos();
                _watcher = new FileSystemWatcher(_cardUserPhotoPath);
                _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | NotifyFilters.FileName;
                _watcher.Created += OnCardUserPhotoPathChanged;
                _watcher.Deleted += OnCardUserPhotoPathChanged;
                _watcher.EnableRaisingEvents = true;
            }
        }

        private void OnCardUserPhotoPathChanged(object sender, FileSystemEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(LoadCardUserPhotos));
        }

        private void LoadCardUserPhotos()
        {            
            CardUserPhotoList.Clear();
            var allImageFiles = Directory.GetFiles(_cardUserPhotoPath).
                                          Select(s => new FileInfo(s)).
                                          Where(f => f.Extension.ToLower() == ".jpeg" || f.Extension.ToLower() == ".jpg").
                                          OrderByDescending(x => x.LastWriteTime);
            foreach (var file in allImageFiles)
                CardUserPhotoList.Add(file.ToString());
        }

        private string _cardUserName = "Фамилия Имя";
        public string CardUserName
        {
            get { return _cardUserName; }
            set
            {
                _cardUserName = value;
                CardUserNameList = new ObservableCollection<string>(_cardUserName.Split(' '));
                RaisePropertyChanged("CardUserName");
            }
        }

        private ObservableCollection<string> _cardUserNameList = new ObservableCollection<string>("Фамилия Имя".Split(' '));
        public ObservableCollection<string> CardUserNameList
        {
            get { return _cardUserNameList; }
            set { _cardUserNameList = value; RaisePropertyChanged("CardUserNameList"); }
        }        

        #endregion

        #region Layout groups

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
                if (AppMode == AppModes.PrintCards)
                    ThreadPool.QueueUserWorkItem(SaveConfiguration);
                RaisePropertyChanged("SelectedLayout");
            }
        }

        #endregion

        #region App settings

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
                        ModeTitle = "Мероприятия";
                        break;
                    case AppModes.ViewLayoutGroup:
                        ModeTitle = SelectedLayoutGroup.Name;
                        break;
                    case AppModes.AddLayoutGroup:
                        ModeTitle = "Добавление мероприятия";
                        break;
                    case AppModes.EditLayoutGroup:
                        ModeTitle = "Изменение мероприятия";
                        break;
                    case AppModes.ViewLayout:
                        ModeTitle = SelectedLayout.Name;
                        break;
                    case AppModes.AddLayout:
                        ModeTitle = "Добавление макета";
                        break;
                    case AppModes.EditLayout:
                        ModeTitle = "Изменение макета";
                        break;
                    case AppModes.PrintCards:
                        ModeTitle = "Печать бейджей";
                        break;
                }
                RaisePropertyChanged("AppMode");
            }
        }

        private string _modeTitle;
        public string ModeTitle
        {
            get { return _modeTitle; }
            set { _modeTitle = value; RaisePropertyChanged("ModeTitle"); }
        }

        private bool _showDeleteConfirmation;
        public bool ShowDeleteConfirmation
        {
            get { return _showDeleteConfirmation; }
            set { _showDeleteConfirmation = value; RaisePropertyChanged("ShowDeleteConfirmation"); }
        }

        #endregion

        public void LoadNewImage(string fileName, string saveName)
        {
            if (SelectedLayout == null) return;
            var objImage = new BitmapImage();
            objImage.BeginInit();
            objImage.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);
            objImage.CacheOption = BitmapCacheOption.OnLoad;
            objImage.EndInit();

            var encoder = new JpegBitmapEncoder();
            var photolocation = Path.Combine(ImagesPath, SelectedLayout.Id + saveName);
            encoder.Frames.Add(BitmapFrame.Create(objImage));
            using (var filestream = new FileStream(photolocation, FileMode.Create))
                encoder.Save(filestream);
            RaisePropertyChanged("SelectedLayout");
        }
    }
}
