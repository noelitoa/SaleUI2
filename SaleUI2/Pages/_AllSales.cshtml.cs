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
    public class _AllSalesModel : PageModel
    {
        private static HttpClient client;
        private IConfiguration _configuration;

        [BindProperty] public SaleEntryGet AllEntries { get; set; }

        public _AllSalesModel(IConfiguration configuration)
        {
            _configuration = configuration;

            client = GetHttpClient();

            SetAllEntries(5);
        }

        public void OnGet()
        {
            
        }

        public async void SetAllEntries(int size)
        {
            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
            AllEntries = await GetAsJson<SaleEntryGet>(uri + "SaleEntry/all/0/" + size + "/time");

        }

        public HttpClient GetHttpClient()
        {
            if (client == null)
            {
                return new HttpClient();
            }

            return client;
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
    }
}