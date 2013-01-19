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

        private BitmapImage _photo;
        public BitmapImage Photo
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
                //TryLoadBackground();
                RaisePropertyChanged("SelectedLayout");
            }
        }
    }
}
