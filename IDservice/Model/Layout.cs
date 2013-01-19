using System;
using System.Xml.Serialization;
using Microsoft.Practices.Prism.ViewModel;

namespace IDservice.Model
{
    public class Layout : NotificationObject, IEditableItem
    {
        public Layout()
        {
            Id = Guid.NewGuid();
        }

        [XmlAttribute]
        public Guid Id { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public double Width { get; set; }
        [XmlAttribute]
        public double Height { get; set; }
        [XmlAttribute]
        public string BackgroundImage { get; set; }
        [XmlAttribute]
        public double PhotoWidth { get; set; }
        [XmlAttribute]
        public double PhotoHeight { get; set; }
        [XmlAttribute]
        public double PhotoX { get; set; }
        [XmlAttribute]
        public double PhotoY { get; set; }

        private Layout _layout;

        public void BeginEdit()
        {
            _layout = new Layout {Id = Id, Name = Name, Width = Width, Height = Height};
        }

        public void EndEdit()
        {
            _layout = null;
        }

        public void CancelEdit()
        {
            if (_layout != null)
            {
                Id = _layout.Id;
                Name = _layout.Name;
                Width = _layout.Width;
                Height = _layout.Height;
                _layout = null;
            }
        }
    }
}
