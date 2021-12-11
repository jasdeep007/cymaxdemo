using cymaxdemo.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cymaxdemo.controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class companyController : ControllerBase
    {
        [Route("company_one")]
        [HttpPost]
        public async Task<returnModelFromCompany1> company_one([FromBody] company_one_data data)
        {
            Random rnd = new Random();
            var result = Task.Run(() => new returnModelFromCompany1() { fair = rnd.Next(1, 10), time = rnd.Next(1, 10), companyName = "Company 1" });
            await Task.WhenAll(result);
            return result.Result;
        }
        [Route("company_two")]
        [HttpPost]
        public async Task<returnModelFromCompany2> company_two([FromBody] company_two_data data)
        {
            Random rnd = new Random();
            var result = Task.Run(() => new returnModelFromCompany2() { amount = rnd.Next(1, 10), time = rnd.Next(1, 10), companyName = "Company 2" });
            await Task.WhenAll(result);
            return result.Result;
        }
        [Route("company_three")]
        [HttpPost]
        public async Task<returnModelFromCompany3> company_three([FromBody] company_three_data data)
        {
            Random rnd = new Random();
            var result = Task.Run(() => new returnModelFromCompany3() { price = rnd.Next(1, 10), time = rnd.Next(1, 10), companyName = "Company 3" });
            await Task.WhenAll(result);
            return result.Result;
        }

        [Route("company_four_withXML")]
        [HttpPost]
        public async Task<string> company_four_withXML([FromBody] company_four_data data)
        {
            Random rnd = new Random();
            var result = Task.Run(() => {
                return @"<root>
                            <quote>"+ rnd.Next(1, 10) + @"</quote>
                            <time>" + rnd.Next(1, 10) + @"</time>
                            <companyName>Company 4</companyName>
                        </root>";
            });
            await Task.WhenAll(result);
            return result.Result;
        }
    }
}
