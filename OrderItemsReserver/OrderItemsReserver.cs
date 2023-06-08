using System;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OrderItemsReserver
{
    public class OrderItemsReserver
    {
        private readonly ILogger _logger;

        public OrderItemsReserver(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<OrderItemsReserver>();
        }

        [Function("OrderItemsReserver")]
        public async Task Run([ServiceBusTrigger("order-items-reserver-queue")] string myQueueItem)
        {
            _logger.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            string data = JsonConvert.DeserializeObject(myQueueItem)!.ToString()!;

            Random random = new Random();
            if (random.Next(3) == 2)
            {
                var client = new HttpClient();

                string jsonData = System.Text.Json.JsonSerializer.Serialize(new
                {
                    email = "Aliaksei_Kuzmitski@epam.com",
                    orderdate = DateTime.Now.ToShortTimeString(),
                    orderdetails = data
                });

                HttpResponseMessage result = await client.PostAsync(
                    "",
                    new StringContent(jsonData, Encoding.UTF8, "application/json"));

                var statusCode = result.StatusCode.ToString();
                return;
            }

            BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=orderitemsreserver2023;AccountKey===;EndpointSuffix=core.windows.net");
            BlobContainerClient container = blobServiceClient.GetBlobContainerClient("mycontainer");
            string time = DateTime.Now.ToLongTimeString();
            BlobClient blob = container.GetBlobClient($"MyBlob{time}");

            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                await blob.UploadAsync(ms);
            }
        }
    }
}
