using cymaxdemo.businesslogic;
using cymaxdemo.models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cymaxdemo.controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class cymaxController : ControllerBase
    {
        private readonly CallAPI callAPI;

        public cymaxController(CallAPI callAPI)
        {
            this.callAPI = callAPI;
        }
        [Route("getbestresult")]
        [HttpPost]
        public async Task<IActionResult> getbestresult([FromBody] getBestResultModel data)
        {
            var company_one_modal = new company_one_data() { source = data.point_source, destination = data.point_destination, dimension = data.dimension };
            var company_two_modal = new company_two_data() { consignee = data.point_source, consignor = data.point_destination, cartons = data.dimension };
            var company_three_modal = new company_three_data() { contactaddress = data.point_source, destinationwarehouseAddress = data.point_destination, packagesDimensions = data.dimension.ToArray() };
            var company_four_modal = @"<xml>
                                        <contactaddress>winnipeg</contactaddress>
                                        <destinationwarehouseAddress>BC</destinationwarehouseAddress>
                                        <dimensions>
                                            <dimension>
                                                <length>1</length>
                                                <width>2</width>
                                                <height>3</height>
                                            </dimension>
                                            <dimension>
                                                <length>11</length>
                                                <width>22</width>
                                                <height>33</height>
                                            </dimension>
                                        </dimensions>
                                    </xml>";
            // call 4 companies api's and get result in terms of fair and time
            var result_from_Company1 = Task.Run(() => callAPI.CallAPIforcompany("/api/company/company_one", company_one_modal));
            var result_from_Company2 = Task.Run(() => callAPI.CallAPIforcompany("/api/company/company_two", company_two_modal));
            var result_from_Company3 = Task.Run(() => callAPI.CallAPIforcompany("/api/company/company_three", company_three_modal));
            var result_from_Company4 = Task.Run(() => callAPI.CallAPIforcompany("/api/company/company_four_withXML", company_four_modal, isxml: true));

            // wait for all api's to be completed // if error, it will return null as handled
            await Task.WhenAll(result_from_Company1, result_from_Company2, result_from_Company3, result_from_Company4);


            List<returnModelFromCompanies> resultList = new List<returnModelFromCompanies>();
            if (result_from_Company1 != null)
                resultList.Add(result_from_Company1.Result);
            if (result_from_Company2 != null)
                resultList.Add(result_from_Company2.Result);
            if (result_from_Company3 != null)
                resultList.Add(result_from_Company3.Result);
            if (result_from_Company4 != null)
                resultList.Add(result_from_Company4.Result);

            var result = getBestOption(resultList);
            if (result != null)
                return Ok(new { bestData = result, totalRecords = resultList.Count });
            else
                return BadRequest("Something went wrong");
        }

        [NonAction]
        public List<returnModelFromCompanies> getBestOption(List<returnModelFromCompanies> dataFromAPIs)
        {
            if (dataFromAPIs.Count > 0)
            {
                dataFromAPIs.RemoveAll(x => x == null);
                dataFromAPIs.Sort((x, y) => (x.fair + x.time).CompareTo(y.fair + y.time));
                List<returnModelFromCompanies> resultdata = new List<returnModelFromCompanies>();
                resultdata.AddRange(dataFromAPIs.Where(x => (x.time + x.fair) == (dataFromAPIs[0].fair + dataFromAPIs[0].time)));
                return resultdata; // Here it gurantees that it contains the best deal
            }
            else
                return null;
        }
    }
}
