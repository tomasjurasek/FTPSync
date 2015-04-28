using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
   [System.Xml.Serialization.XmlTypeAttribute]
   public  class Item
   {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Key { get; set; }
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value { get; set; }
   }
    [System.Xml.Serialization.XmlTypeAttribute]
    [System.Xml.Serialization.XmlRootAttribute]
    public class Configuration
    {
        [System.Xml.Serialization.XmlElementAttribute("Item")]
        public Item[] Item { get; set; }
    }
}
