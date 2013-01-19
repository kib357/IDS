using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using Microsoft.Practices.Prism.ViewModel;

namespace IDservice.Model
{
    public class LayoutGroup : NotificationObject, IEditableItem
    {
        public LayoutGroup()
        {
            Id = Guid.NewGuid();
        }

        private LayoutGroup _layoutGroup;

        [XmlAttribute]
        public Guid Id { get; set; }
        
        private string _name;
        [XmlAttribute]
        public string Name { get { return _name; } set { _name = value; RaisePropertyChanged("Name"); } }

        [XmlArray]
        public ObservableCollection<Layout> Layouts = new ObservableCollection<Layout>();

        public void BeginEdit()
        {
            _layoutGroup = new LayoutGroup {Id = Id, Name = Name};
        }

        public void EndEdit()
        {
            _layoutGroup = null;
        }

        public void CancelEdit()
        {
            if (_layoutGroup != null)
            {
                Id = _layoutGroup.Id;
                Name = _layoutGroup.Name;
            }
        }
    }
}
