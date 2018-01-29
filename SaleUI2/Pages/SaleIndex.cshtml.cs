using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SaleUI2.Models;

namespace SaleUI2.Pages
{
    public class SaleIndexModel : PageModel
    {
        private static HttpClient client;
        private IConfiguration _configuration;

        public SaleIndexModel(IConfiguration configuration)
        {
            _configuration = configuration;

            client = GetHttpClient();
        }

        public async Task OnGet(string id)
        {
            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
            SaleEntries = await GetAsJson<List<SaleEntry>>(uri + "SaleEntry/saleentry/" + id);
            SaleEntry = SaleEntries.FirstOrDefault();

        }

        public async Task<IActionResult> OnPostUpdateAsync(SaleEntry saleEntry)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
            saleEntry.TimeStamp = DateTime.Now;

            var stringContent = new StringContent(JsonConvert.SerializeObject(saleEntry), Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync(uri + "saleentry/" + saleEntry.Id, stringContent);
            if (response.IsSuccessStatusCode)
            {
                var location = response.Headers.GetValues("location").FirstOrDefault();
                if (location != null)
                {
                    var id = location.Substring(location.LastIndexOf('/') + 1);
                    return RedirectToPage("/SaleIndex?id=" + id);
                }

                return RedirectToPage("/SaleIndex");
            }

            return RedirectToPage("/Error");
        }

        public async Task<IActionResult> OnPostAsync(SaleEntry saleEntry)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
            saleEntry.TimeStamp = DateTime.Now;

            var stringContent = new StringContent(JsonConvert.SerializeObject(saleEntry), Encoding.UTF8,
                "application/json");
            var response = await client.PostAsync(uri + "saleentry", stringContent);
            if (response.IsSuccessStatusCode)
            {
                var location = response.Headers.GetValues("location").FirstOrDefault();
                if (location != null)
                {
                    var id = location.Substring(location.LastIndexOf('/') + 1);
                    return RedirectToPage("/SaleIndex?id=" + id);
                }

                return RedirectToPage("/SaleIndex");
            }

            return RedirectToPage("/Error");
        }

        [BindProperty]
        public SaleEntry SaleEntry { get; set; }

        [BindProperty]
        public List<SaleEntry> SaleEntries { get; set; }


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