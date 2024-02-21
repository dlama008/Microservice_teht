using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microservice_teht;

namespace Microservice_teht.Services
{
    public class ElectricityPriceFetchingService : BackgroundService
    {
        private readonly ILogger<ElectricityPriceFetchingService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public ElectricityPriceFetchingService(ILogger<ElectricityPriceFetchingService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await FetchElectricityPricesAsync(stoppingToken);
                //Wait 1 hour to fetch again
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task FetchElectricityPricesAsync(CancellationToken stoppingToken)
        {
            var httpClient = _httpClientFactory.CreateClient();
            try
            {
                var response = await httpClient.GetAsync(Constants.Constants.PorssisahkoUrl, stoppingToken);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Hinnat haettu: {content}");


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Virhe sähkön hintatietojen haussa");
            }
        }


    }
}
