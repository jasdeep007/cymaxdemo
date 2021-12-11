using cymaxdemo.models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace cymaxdemo.businesslogic
{
    public class CallAPI
    {
        private IHttpContextAccessor httpContextAccessor;

        public CallAPI(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> CallAPIforcompany(string url, object model, bool isxml = false)
        {
            using var netclient = new HttpClient();
            string json = await Task.Run(() => JsonSerializer.Serialize(model));
            var httpContent = isxml == false ? new StringContent(json, Encoding.UTF8, "application/json")
                                             : new StringContent(model.ToString(), Encoding.UTF8, "text/xml"); ;

            var request = httpContextAccessor.HttpContext.Request;
            var domain = $"{request.Scheme}://{request.Host}";

            using var httpResponse = await netclient.PostAsync(domain + url, httpContent);
            if (httpResponse.Content != null)
            {
                try
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    // return JsonSerializer.Deserialize<object>(responseContent);
                    return responseContent;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
