using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace cymaxdemo.models
{   
    public class getBestResultModel
    {
        [Required]
        public string point_source { get; set; }
        [Required]
        public string point_destination { get; set; }
        [Required]
        public List<dimension> dimension { get; set; }
    }
    public class dimension
    {
        [Required]
        public int length { get; set; }
        [Required]
        public int width { get; set; }
        [Required]
        public int height { get; set; }
    }
    public class returnModelFromCompanies
    {
        public int fair { get; set; }
        public int time { get; set; }
        public string companyName { get; set; }
    }
    public class basereturnmodel
    {
        public string companyName { get; set; }
        public int time { get; set; }
    }

    public class returnModelFromCompany1 : basereturnmodel
    {        
        public int fair { get; set; }                
    }
    public class returnModelFromCompany2 : basereturnmodel
    {       
        public int amount { get; set; }
    }
    public class returnModelFromCompany3 : basereturnmodel
    {       
        public int price { get; set; }
    }
    public class returnModelFromCompany4_xml
    {
        public returnModelFromCompany4 root { get; set; }
    }
    public class returnModelFromCompany4 : basereturnmodel
    {       
        public int quote { get; set; }
    }
}
