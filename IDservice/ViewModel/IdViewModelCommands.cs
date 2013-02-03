using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDservice.Model;
using Microsoft.Practices.Prism.Commands;

namespace IDservice.ViewModel
{
    public partial class IdViewModel
    {
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
                var backgroundPath = Path.Combine(ImagesPath, SelectedLayout.Id + "_background.jpg");
                var othersidePath = Path.Combine(ImagesPath, SelectedLayout.Id + "_otherside.jpg");
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

        private void DeletePreview()
        {
            ShowDeleteConfirmation = true;
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

        private void Cancel()
        {
            if (SelectedLayoutGroup != null)
                SelectedLayoutGroup.CancelEdit();
            if (SelectedLayout != null)
                SelectedLayout.CancelEdit();
            AppMode = _prevAppMode;
        }        

        private void PrintCards()
        {
            AppMode = AppModes.PrintCards;
            if (Layouts != null)
                SelectedLayout = Layouts.FirstOrDefault();
        }
    }
}
