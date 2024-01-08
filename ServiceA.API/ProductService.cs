using ServiceA.API.Models;

namespace ServiceA.API
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductService> _logger;

        public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Product> GetProduct(int id)
        {
            //program.cs teki addhttpclientaki base adresin sonuna eklenecek bu id istek yapınca
            var product = await _httpClient.GetFromJsonAsync<Product>($"{id}");
            _logger.LogInformation($"Product : {product.Id}-{product.Name}");

            return product;
        }
    }
}