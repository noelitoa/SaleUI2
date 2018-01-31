using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SaleUI2.Models;

namespace SaleUI2.Pages
{
    public class DashboardModel : PageModel
    {

        private static HttpClient client;
        private IConfiguration _configuration;

        public SaleEntryGet AllEntries { get; set; }

        public DashboardModel(IConfiguration configuration)
        {
            _configuration = configuration;

            client = GetHttpClient();

            
        }

        public void OnGet()
        {
            SetAllEntries(5);
        }

        public async void SetAllEntries(int size)
        {
            AllEntries = new SaleEntryGet(){ SaleEntryGets = new List<SaleEntry>()};
            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
            this.AllEntries = GetAsJsonSync<SaleEntryGet>(uri + "SaleEntry/all/0/" + size + "/time");

        }

        private async Task<T> GetAsJson<T>(string requestUri)
        {
            var jsonResponse = await client.GetAsync(requestUri);

            using (var content = jsonResponse.Content)
            {
                var json = content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json.Result);
            }
        }

        private T GetAsJsonSync<T>(string requestUri)
        {
            var response = client.GetAsync(requestUri).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                var responseString =  responseContent.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<T>(responseString);
            }

            return default(T);
        }

        public HttpClient GetHttpClient()
        {
            if (client == null)
            {
                return new HttpClient();
            }

            return client;
        }
    }
}