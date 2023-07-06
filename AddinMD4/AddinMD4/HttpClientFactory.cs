using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AddinMD4
{
    public interface IHttpClientFactory<TEntity> : IDisposable where TEntity : Entity
    {


    }

  
    internal sealed class HttpClientFactory<TEntity> : IHttpClientFactory<TEntity> where TEntity : Entity, new()
    {
        private readonly HttpClient _httpClient;
        public HttpClientFactory(HttpClient httpClient) { _httpClient = httpClient; }
       // const string uri = "http://localhost:5064/contador";//posso colocar setting
        Uri clientBaseAddress = new Uri("http://localhost:5064/contador");//colocar build

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<TEntity> GetInvoiceItens(int  page, int take)
        {
           // var uri = API.Sales.GetAllInvoiceItens(page, take);
            var responseString = await _httpClient.GetStringAsync(clientBaseAddress);
            return JsonConvert.DeserializeObject<TEntity>(responseString);

        }
    }
}
