using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDservice.Model
{
    public class Layout
    {
        public Layout()
        {
            Id = Guid.NewGuid();
        }

        [XmlAttribute]
        public Guid Id { get; set; }
        [XmlAttribute]
        public double Width { get; set; }
        [XmlAttribute]
        public double Height { get; set; }
    }
}
