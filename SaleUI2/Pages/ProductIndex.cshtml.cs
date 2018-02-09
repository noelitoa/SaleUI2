using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using SaleUI2.Models;
using X.PagedList;

namespace SaleUI2.Pages
{
    public class ProductIndexModel : PageModel
    {
        private static HttpClient client;
        private IConfiguration _configuration;

        private const int DefaultPageSize = 20;

        [BindProperty]
        public Product Product { get; set; }
        [BindProperty]
        public List<Product> Products { get; set; }
        [BindProperty]
        public IPagedList<Product> PageProducts { get; set; }

        public ProductIndexModel(IConfiguration configuration)
        {
            _configuration = configuration;

            client = GetHttpClient();
        }
        
        public void OnGet(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
                var products = GetAsJsonSync<List<Product>>(uri + "Product/product/" + id);
                Product = products.FirstOrDefault();
            }
            else
            {
                var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
                Products = GetAsJsonSync<List<Product>>(uri + "Product/all/0/999/productSKU.keyword/0");
                PageProducts = Products.ToPagedList(1, DefaultPageSize);
            }
        }
        
        public void OnGetPaging(int id)
        {
                var uri = _configuration.GetSection("SaleEsApi").GetSection("Uri").Value;
                Products = GetAsJsonSync<List<Product>>(uri + "Product/all/0/999/productSKU.keyword/0");
                PageProducts = Products.ToPagedList(id, DefaultPageSize);
            
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

        #endregion
    }
}