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
            _name = "Новый макет";
            _width = 188.9764;
            _height = 188.9764;           
            _photoWidth = 75.5906;
            _photoHeight = 75.5906;
            _photoX = 0;
            _photoY = 0;
            _nameX = 0;
            _nameY = 0;
            _nameFontSize = 8;
        }

        [XmlAttribute]
        public Guid Id { get; set; }
        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged("Name"); }
        }

        [XmlAttribute]
        public double Width
        {
            get { return _width; }
            set
            {
                if (value < _width)
                {
                    var ratio = value / _width;
                    ResizeElemenetsByRatio(ratio);
                }
                _width = value;
                RaisePropertyChanged("Width");
            }
        }

        [XmlAttribute]
        public double Height
        {
            get { return _height; }
            set
            {
                if (value < _height)
                {
                    var ratio = value / _height;
                    ResizeElemenetsByRatio(ratio);
                }
                _height = value;
                RaisePropertyChanged("Height");
            }
        }

        private void ResizeElemenetsByRatio(double ratio)
        {
            NameFontSize *= ratio;
            PhotoWidth *= ratio;
            PhotoHeight *= ratio;
            PhotoX *= ratio;
            PhotoY *= ratio;
            NameX *= ratio;
            NameY *= ratio;
        }

        [XmlAttribute]
        public string BackgroundImage { get; set; }
        [XmlAttribute]
        public double PhotoWidth
        {
            get { return _photoWidth; }
            set { _photoWidth = value; RaisePropertyChanged("PhotoWidth"); }
        }

        [XmlAttribute]
        public double PhotoHeight
        {
            get { return _photoHeight; }
            set { _photoHeight = value; RaisePropertyChanged("PhotoHeight"); }
        }

        [XmlAttribute]
        public double PhotoX
        {
            get { return _photoX; }
            set { _photoX = value; RaisePropertyChanged("PhotoX"); }
        }

        [XmlAttribute]
        public double PhotoY
        {
            get { return _photoY; }
            set { _photoY = value; RaisePropertyChanged("PhotoY"); }
        }

        [XmlAttribute]
        public double NameX
        {
            get { return _nameX; }
            set { _nameX = value; RaisePropertyChanged("NameX"); }
        }

        [XmlAttribute]
        public double NameY
        {
            get { return _nameY; }
            set { _nameY = value; RaisePropertyChanged("NameY"); }
        }

        [XmlAttribute]
        public double NameFontSize
        {
            get { return _nameFontSize; }
            set { _nameFontSize = value; RaisePropertyChanged("NameFontSize"); }
        }

        private Layout _layout;
        private double _width;
        private double _height;
        private double _photoWidth;
        private double _photoHeight;
        private double _photoX;
        private double _photoY;
        private double _nameX;
        private double _nameY;
        private double _nameFontSize;
        private string _name;

        public void BeginEdit()
        {
            _layout = new Layout { Id = Id, Name = Name, Width = Width, Height = Height };
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
