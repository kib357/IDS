using System;
using System.Windows.Media;
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
            _nameWidth = 90;
            _nameHeight = 40;
            _nameX = 0;
            _nameY = 0;
            _nameColor = Color.FromRgb(0, 0, 0);
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
                if (value < 0) return;
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
                if (value < 0) return;
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
            PhotoWidth *= ratio;
            PhotoHeight *= ratio;
            PhotoX *= ratio;
            PhotoY *= ratio;
            NameWidth *= ratio;
            NameHeight *= ratio;
            NameX *= ratio;
            NameY *= ratio;
        }

        [XmlAttribute]
        public string BackgroundImage { get; set; }
        [XmlAttribute]
        public double PhotoWidth
        {
            get { return _photoWidth; }
            set { _photoWidth = Math.Max(0, value); RaisePropertyChanged("PhotoWidth"); }
        }

        [XmlAttribute]
        public double PhotoHeight
        {
            get { return _photoHeight; }
            set { _photoHeight = Math.Max(0, value); RaisePropertyChanged("PhotoHeight"); }
        }

        [XmlAttribute]
        public double PhotoX
        {
            get { return _photoX; }
            set { _photoX = Math.Max(0, value); RaisePropertyChanged("PhotoX"); }
        }

        [XmlAttribute]
        public double PhotoY
        {
            get { return _photoY; }
            set { _photoY = Math.Max(0, value); RaisePropertyChanged("PhotoY"); }
        }

        [XmlAttribute]
        public double NameX
        {
            get { return _nameX; }
            set { _nameX = Math.Max(0, value); RaisePropertyChanged("NameX"); }
        }

        [XmlAttribute]
        public double NameY
        {
            get { return _nameY; }
            set { _nameY = Math.Max(0, value); RaisePropertyChanged("NameY"); }
        }

        [XmlAttribute]
        public double NameWidth
        {
            get { return _nameWidth; }
            set { _nameWidth = Math.Max(0, value); RaisePropertyChanged("NameWidth"); }
        }

        [XmlAttribute]
        public double NameHeight
        {
            get { return _nameHeight; }
            set { _nameHeight = Math.Max(0, value); RaisePropertyChanged("NameHeight"); }
        }

        [XmlAttribute]
        private string SerializedColor
        {
            get { return NameColor.ToString(); }
            set
            {
                try
                {
                    byte a = Convert.ToByte(value.Substring(1, 2));
                    byte r = Convert.ToByte(value.Substring(3, 2));
                    byte g = Convert.ToByte(value.Substring(5, 2));
                    byte b = Convert.ToByte(value.Substring(7, 2));
                    NameColor = Color.FromArgb(a, r, g, b);
                }
                catch (Exception)
                {
                    NameColor = Color.FromRgb(0, 0, 0);
                }

            }
        }

        public Color NameColor
        {
            get { return _nameColor; }
            set { _nameColor = value; RaisePropertyChanged("NameColor"); RaisePropertyChanged("NameBrush"); }
        }

        public SolidColorBrush NameBrush
        {
            get { return new SolidColorBrush(NameColor);}
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
        private double _nameWidth;
        private double _nameHeight;
        private Color _nameColor;
        private string _name;

        public void BeginEdit()
        {
            _layout = new Layout { Id = Id,
                                   Name = Name,
                                   Width = Width,
                                   Height = Height,
                                   PhotoWidth = PhotoWidth,
                                   PhotoHeight = PhotoHeight,
                                   PhotoX = PhotoX,
                                   PhotoY = PhotoY,
                                   NameWidth = NameWidth,
                                   NameHeight = NameHeight,
                                   NameX = NameX,
                                   NameY = NameY,
                                   NameColor = NameColor
            };
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
                PhotoWidth = _layout.PhotoWidth;
                PhotoHeight = _layout.PhotoHeight;
                PhotoX = _layout.PhotoX;
                PhotoY = _layout.PhotoY;
                NameWidth = _layout.NameWidth;
                NameHeight = _layout.NameHeight;
                NameX = _layout.NameX;
                NameY = _layout.NameY;
                NameColor = _layout.NameColor;
                _layout = null;
            }
        }
    }
}
