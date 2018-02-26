using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SaleUI2.Models;
using X.PagedList;

namespace SaleUI2.Pages
{
    public class InventoryIndexModel : PageModel
    {

        private static HttpClient client;
        private IConfiguration _configuration;

        private const int DefaultPageSize = 20;

        [BindProperty]
        public Inventory Inventory { get; set; }
        [BindProperty]
        public string ConfirmMessage { get; set; }

        public List<Inventory> Inventories { get; set; }
        public IPagedList<Inventory> PageInventories { get; set; }



        public InventoryIndexModel(IConfiguration configuration)
        {
            _configuration = configuration;

            client = GetHttpClient();
        }


        public void OnGet(string id, string confirm)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
                var inventories = GetAsJsonSync<List<Inventory>>(uri + "Inventory/inventory/" + id);
                Inventory = inventories.FirstOrDefault();

                if (!String.IsNullOrEmpty(confirm) && confirm == "ok")
                {
                    ConfirmMessage = $"Update successful to id = {id}.";
                }
            }
            else
            {
                var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
                Inventories = GetAsJsonSync<List<Inventory>>(uri + "Inventory/all/0/9999/quantity/0");
                PageInventories = Inventories.ToPagedList(1, DefaultPageSize);
            }
        }

        public void OnGetPaging(int id)
        {
            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
            Inventories = GetAsJsonSync<List<Inventory>>(uri + "Inventory/all/0/9999/quantity/0");
            PageInventories = Inventories.ToPagedList(id, DefaultPageSize);

        }


        public async Task<IActionResult> OnPostUpdateAsync(Inventory inventory)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
            inventory.TimeStamp = DateTime.UtcNow;

            var stringContent = new StringContent(JsonConvert.SerializeObject(inventory), Encoding.UTF8,
                "application/json");
            var response = await client.PutAsync(uri + "inventory/" + inventory.Id, stringContent);

            if (response.IsSuccessStatusCode)
            {
                var location = response.Headers.GetValues("location").FirstOrDefault();
                if (location != null)
                {
                    var _itemId = location.Substring(location.LastIndexOf('/') + 1);
                    return RedirectToPage("/InventoryIndex", new { id = _itemId, confirm = "ok" });
                }

                return RedirectToPage("/Error");
            }

            return RedirectToPage("/Error");
        }

        public void OnPostSearchAsync(string query)
        {
            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;

            Inventories = GetAsJsonSync<List<Inventory>>(uri + "Inventory/search/" + query + "/productSKU.keyword/0");
            PageInventories = Inventories.ToPagedList(1, DefaultPageSize);

        }

        public void OnGetSearchAsync(string q)
        {
            var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;

            Inventories = GetAsJsonSync<List<Inventory>>(uri + "Inventory/search/" + q + "/productSKU.keyword/0");
            PageInventories = Inventories.ToPagedList(1, DefaultPageSize);

        }

        public async Task<IActionResult> OnGetDelete(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
                var response = await DeleteAsString(uri + "inventory/" + id);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var itemId = id;
                    return RedirectToPage("/InventoryIndex");
                }
            }

            return RedirectToPage("/Error");
        }

        public async Task<IActionResult> OnPostDelete(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
                var response = await DeleteAsString(uri + "inventory/" + id);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var itemId = id;
                    return RedirectToPage("/InventoryIndex");
                }
            }

            return RedirectToPage("/Error");
        }

        #region Private

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

        private T GetAsJsonSync<T>(string requestUri)
        {
            var response = client.GetAsync(requestUri).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                var responseString = responseContent.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<T>(responseString);
            }

            return default(T);
        }

        private async Task<HttpResponseMessage> DeleteAsString(string requestUri)
        {
            var jsonResponse = await client.DeleteAsync(requestUri);

            return jsonResponse;
        }
        #endregion
    }
}