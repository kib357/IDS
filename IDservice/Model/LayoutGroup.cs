using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDservice.Model
{
    public class LayoutGroup
    {
        public LayoutGroup()
        {
            Id = Guid.NewGuid();
        }

        [XmlAttribute]
        public Guid Id { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlArray]
        public ObservableCollection<Layout> Layouts = new ObservableCollection<Layout>();

        public void PartialClone(LayoutGroup groupToCloneInto)
        {
            groupToCloneInto.Id = Id;
            groupToCloneInto.Name = Name;
        }
    }
}
