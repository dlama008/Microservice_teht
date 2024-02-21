using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microservice_teht.Constants;
using Microservice_Teht.Models;
using Microservice_Teht.Data;

namespace Microservice_Teht.Controllers
{

    public class PricesContainer
    {
        public List<ElectricityPriceInfo> Prices { get; set; }
    }

    public class ElectricityPriceInfo
    {
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }


    [Route("api/[controller]")]
    [ApiController]
    public class ElectricityController : ControllerBase
    {
        private readonly ElectricityPriceDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ElectricityController(ElectricityPriceDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }
        //HAE SÄHKÖ
        [HttpGet("GetSahko")]
        public async Task<IActionResult> GetLatestPrices()
        {
            try
            {
                using var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(Constants.PorssisahkoUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // Deserialisoi vastaanotettu JSON-data
                var pricesContainer = JsonConvert.DeserializeObject<PricesContainer>(content);

                // Tässä kohtaa voit tehdä jotain datalle, esimerkiksi tulostaa sen
                foreach (var priceInfo in pricesContainer.Prices)
                {
                    Console.WriteLine($"Price: {priceInfo.Price}, StartDate: {priceInfo.StartDate}, EndDate: {priceInfo.EndDate}");
                }

                // Lähetä data toiselle palvelulle
                await SendDataToOtherService(content, "https://localhost:7034/api/ElectricityData");

                // Palauta alkuperäinen vastaus tai muokattu data
                return Ok(content);
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, "Virhe datan haussa: " + e.Message);
            }
        }

        //LÄHETÄ DATA
        private async Task SendDataToOtherService(string content, string url)
        {
            var stringContent = new StringContent(content, Encoding.UTF8, "application/json");

            try
            {
                using var client = _httpClientFactory.CreateClient();

                var response = await client.PostAsync(url, stringContent);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Server response: {responseContent}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error sending request: {e.Message}");
            }
        }
    }
}
