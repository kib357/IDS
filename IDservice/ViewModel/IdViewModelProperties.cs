using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;
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
                _prevAppMode = _appMode;
                _appMode = value;
                switch (_appMode)
                {
                    case AppModes.LayoutGroups:
                        Title = "Мероприятия";
                        break;
                    case AppModes.ViewLayoutGroup:
                        Title = SelectedItem.Name;
                        break;
                    case AppModes.AddLayoutGroup:
                        Title = "Добавление мероприятия";
                        break;
                    case AppModes.EditLayoutGroup:
                        Title = "Изменение мероприятия";
                        break;
                    case AppModes.ViewLayout:
                        Title = SelectedItem.Name;
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

        private BitmapImage _background;
        public BitmapImage Background
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

        private IEditableItem _selectedItem;
        public IEditableItem SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; RaisePropertyChanged("SelectedItem"); }
        }

        public Layout SelectedLayout { get; set; }
    }
}
