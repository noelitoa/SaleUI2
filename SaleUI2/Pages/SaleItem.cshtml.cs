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
using Serilog;

namespace SaleUI2.Pages
{
    public class SaleItemModel : PageModel
    {
        private static HttpClient client;
        private IConfiguration _configuration;

        [BindProperty]
        public List<SaleEntry> SaleEntries { get; set; }

        public SaleItemModel(IConfiguration configuration)
        {
            _configuration = configuration;

            client = GetHttpClient();
        }

        //public void OnGet(string id)
        //{
        //    var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
        //    var response = client.GetAsync(uri + "saleentry" + id);
        //    response.Issu
            



        //}

        public async Task OnGet(string id)
        {
            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
            //var response = client.GetAsync(uri + "Saleentry/saleentry/" + id);

            //var x = response.Result.Content.ReadAsStringAsync();

            //JsonConvert.DeserializeObject<SaleEntry>(x);

            SaleEntries = await GetAsJson<List<SaleEntry>>(uri + "SaleEntry/saleentry/" + id);

            //JsonConvert.DeserializeObject<SaleEntry>(y);
        }

        public async Task<T> GetAsJson<T>(string requestUri)
        {
            var jsonResponse = await client.GetAsync(requestUri);

            Log.Information($"Get Response = {jsonResponse}");

            using (var content = jsonResponse.Content)
            {
                var json = content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json.Result);
            }
        }

        private HttpClient GetHttpClient()
        {
            if (client == null)
            {
                return new HttpClient();
            }

            return client;
        }
    }
}