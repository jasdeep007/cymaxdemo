using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace cymaxdemo.models
{
    public class company_one_data
    {
        public string source { get; set; }
        public string destination { get; set; }
        public List<dimension> dimension { get; set; }
    }
    public class company_two_data
    {
        public string consignee { get; set; }
        public string consignor { get; set; }
        public List<dimension> cartons { get; set; }
    }
    public class company_three_data
    {
        public string contactaddress { get; set; }
        public string destinationwarehouseAddress { get; set; }
        public dimension[] packagesDimensions { get; set; }
    }

    [XmlRoot(ElementName = "xml")]
    public class company_four_data
    {
        [XmlElement]
        public string contactaddress { get; set; }
        [XmlElement]
        public string destinationwarehouseAddress { get; set; }
        [XmlArray]
        public List<dimension> dimensions { get; set; }
    }   
}
