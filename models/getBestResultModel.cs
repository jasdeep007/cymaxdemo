using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cymaxdemo.models
{
    public class getBestResultModel
    {
        public string point_source { get; set; }
        public string point_destination { get; set; }
        public List<dimension> dimension { get; set; }
    }
    public class dimension
    {
        public int length { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }
    public class returnModelFromCompanies
    {
        public int fair { get; set; }
        public int time { get; set; }
        public string companyName { get; set; }
    }
}
