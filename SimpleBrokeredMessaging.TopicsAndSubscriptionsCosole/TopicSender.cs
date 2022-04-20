using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.TopicsAndSubscriptionsCosole
{
    internal class TopicSender
    {
        private ServiceBusSender _serviceBusSender;

        public TopicSender(string connectionString, string topicName)
        {
            var client = new ServiceBusClient(connectionString);
            _serviceBusSender = client.CreateSender(topicName);
        }

        public async Task SendOrderMessage(Order order)
        {
            Console.WriteLine($"{order.ToString()}");

            // Serialize the order to JSON
            var orderJson = JsonConvert.SerializeObject(order);

            // Create a message containing the serialized order Json
            var message = new ServiceBusMessage(orderJson);

            // Promote properties ...
            message.ApplicationProperties.Add("region", order.Region);
            message.ApplicationProperties.Add("items", order.Items);
            message.ApplicationProperties.Add("value", order.Value);
            message.ApplicationProperties.Add("loyalty", order.HasLoyltyCard);

            // Set the correlation id
            message.CorrelationId = order.Region;

            // Send the message
            await _serviceBusSender.SendMessageAsync(message);
        }

        public async Task Close()
        {
            await _serviceBusSender.CloseAsync();

        }
    }
}
