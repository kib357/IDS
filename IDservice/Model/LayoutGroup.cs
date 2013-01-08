using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDservice.Model
{
    public class LayoutGroup : IEditableItem
    {
        public LayoutGroup()
        {
            Id = Guid.NewGuid();
        }

        private LayoutGroup _layoutGroup;

        [XmlAttribute]
        public Guid Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

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
