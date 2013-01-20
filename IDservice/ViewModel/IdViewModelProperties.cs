﻿using System.Collections.ObjectModel;
using System.Printing;
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
            set { _selectedPrinter = value; RaisePropertyChanged("SelectedPrinter"); }
        }

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
