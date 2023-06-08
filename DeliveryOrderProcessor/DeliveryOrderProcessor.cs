using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderItemsReserver.Models;
using System.Net;

namespace DeliveryOrderProcessor
{
    public class DeliveryOrderProcessor
    {
        private readonly ILogger _logger;

        public DeliveryOrderProcessor(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<DeliveryOrderProcessor>();
        }

        [Function("DeliveryOrderProcessor")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Order orderFromSite = JsonConvert.DeserializeObject<Order>(requestBody)!;
            orderFromSite.CalculateTotalSum();

            var data = JsonConvert.SerializeObject(orderFromSite);

            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString($"Welcome to Azure Functions! Your order was stored in the system. \n{data}");


            using CosmosClient client = new("AccountEndpoint=https://azurecosmosdb2023.documents.azure.com:443/;AccountKey===;");

            Container container = client.GetContainer("orderdb", "ordercontainer");

            var orderRecord = new OrderRecord(orderFromSite.Id.ToString(), string.Join(" ", orderFromSite.OrderItems!.Select(x => x.ItemOrdered!.ProductName)), orderFromSite.TotalSum);

            OrderRecord createdItem = await container.CreateItemAsync(item: orderRecord);

            return response;
        }
    }

        public record OrderRecord(
            string id,
            string itemsList,
            double total
    );
}
