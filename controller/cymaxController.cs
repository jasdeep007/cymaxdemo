using cymaxdemo.businesslogic;
using cymaxdemo.models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;

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

        /// <summary>
        /// POST API to handle the request from the user to fetch data from different companies and returns the best list out of, it in terms of Amount and time
        /// </summary>
        /// <param name="data">getBestResultModel model accepted as a parameter in the body</param>
        /// <returns>HttpResponse with either OK or BadRequest</returns>
        [Route("getbestresult")]
        [HttpPost]
        public async Task<IActionResult> getbestresult([FromBody] getBestResultModel data)
        {
            try
            {
                #region Checking Model for errors
                if (!ModelState.IsValid)
                {
                    return BadRequest("Model is not valid");
                }
                #endregion

                #region Setting Different Models required by various API's
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
                #endregion

                #region call 4 companies api's and get result in their own formats
                var result_from_Company1 = Task.Run(() => callAPI.CallAPIforcompany("/api/company/company_one", company_one_modal));
                var result_from_Company2 = Task.Run(() => callAPI.CallAPIforcompany("/api/company/company_two", company_two_modal));
                var result_from_Company3 = Task.Run(() => callAPI.CallAPIforcompany("/api/company/company_three", company_three_modal));
                var result_from_Company4 = Task.Run(() => callAPI.CallAPIforcompany("/api/company/company_four_withXML", company_four_modal, isxml: true));
                #endregion

                #region wait for all api's to be completed // if error, it will return null as handled
                await Task.WhenAll(result_from_Company1, result_from_Company2, result_from_Company3, result_from_Company4);
                #endregion

                #region Deserializing APIs results as per their own format models and converting into one common model
                List<returnModelFromCompanies> resultList = new List<returnModelFromCompanies>();
                if (result_from_Company1 != null)
                {
                    resultList.Add(Mapclass(JsonSerializer.Deserialize<returnModelFromCompany1>(result_from_Company1.Result)));
                }
                if (result_from_Company2 != null)
                {
                    resultList.Add(Mapclass(JsonSerializer.Deserialize<returnModelFromCompany2>(result_from_Company2.Result)));
                }
                if (result_from_Company3 != null)
                {
                    resultList.Add(Mapclass(JsonSerializer.Deserialize<returnModelFromCompany3>(result_from_Company3.Result)));
                }
                if (result_from_Company4 != null)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result_from_Company4.Result);
                    var options = new JsonSerializerOptions()
                    {
                        NumberHandling = JsonNumberHandling.AllowReadingFromString |
                                         JsonNumberHandling.WriteAsString
                    };
                    string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc);
                    var obj = JsonSerializer.Deserialize<returnModelFromCompany4_xml>(json, options);
                    resultList.Add(Mapclass(obj));
                }
                #endregion

                #region Getting the best result in terms of Price
                var result = getBestOption(resultList);
                #endregion

                #region Returning the result Either OK or BadRequest
                if (result != null)
                    return Ok(new { bestData = result, totalRecords = resultList.Count });
                else
                    return BadRequest("Something went wrong"); 
                #endregion
            }
            catch (Exception)
            {
                return BadRequest("Something went wrong");
            }
        }

        /// <summary>
        /// Method to map different API responses to common model "returnModelFromCompanies"
        /// </summary>
        /// <param name="returnModelFromCompanyAPI">Object as a response from Company API, that can be from any company API</param>
        /// <returns>returnModelFromCompanies model (common model)</returns>
        [NonAction]
        public returnModelFromCompanies Mapclass(object returnModelFromCompanyAPI)
        {
            returnModelFromCompanies returnobject = new returnModelFromCompanies();
            if (typeof(returnModelFromCompany1) == returnModelFromCompanyAPI.GetType())
            {
                var result = (returnModelFromCompany1)returnModelFromCompanyAPI;
                returnobject.companyName = result.companyName;
                returnobject.time = result.time;
                returnobject.fair = result.fair;
            }
            else if (typeof(returnModelFromCompany2) == returnModelFromCompanyAPI.GetType())
            {
                var result = (returnModelFromCompany2)returnModelFromCompanyAPI;
                returnobject.companyName = result.companyName;
                returnobject.time = result.time;
                returnobject.fair = result.amount;
            }
            else if (typeof(returnModelFromCompany3) == returnModelFromCompanyAPI.GetType())
            {
                var result = (returnModelFromCompany3)returnModelFromCompanyAPI;
                returnobject.companyName = result.companyName;
                returnobject.time = result.time;
                returnobject.fair = result.price;
            }
            else if (typeof(returnModelFromCompany4_xml) == returnModelFromCompanyAPI.GetType())
            {
                var result = (returnModelFromCompany4_xml)returnModelFromCompanyAPI;
                returnobject.companyName = result.root.companyName;
                returnobject.time = result.root.time;
                returnobject.fair = result.root.quote;
            }
            return returnobject;
        }

        /// <summary>
        /// Finds the best result from different APIs responses by taking care of Amount and Price
        /// </summary>
        /// <param name="dataFromAPIs">List of data from all companies API</param>
        /// <returns>List of "returnModelFromCompanies" model that contains the minimum unit in term of both amount and time</returns>
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
