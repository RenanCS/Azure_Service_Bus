using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Threading.Tasks;

namespace SimpleBorkeredMessaging.ManagementConsole
{
    internal class ManagementHelper
    {
        private ServiceBusAdministrationClient _serviceBusAdministrationClient;

        public ManagementHelper(string connectionString)
        {
            // Using the specified credentials
            _serviceBusAdministrationClient = new ServiceBusAdministrationClient(connectionString);
        }

        public async Task CreateQueueAsync(string queueName)
        {
            Console.Write("Creating queue {0} ...", queueName);

            var createQueueOption = GetCreateQueueOption(queueName);
            var response = await _serviceBusAdministrationClient.CreateQueueAsync(createQueueOption);
            var queueProperties = response.Value;

            Console.WriteLine("Done!");
        }

        public async Task DeleteQueueAsync(string queueName)
        {
            Console.Write("Deleting queue {0} ...", queueName);

            await _serviceBusAdministrationClient.DeleteQueueAsync(queueName);

            Console.WriteLine("Done!");
        }

        public async Task ListQueuesAsync()
        {
            var queuePropertiesList = _serviceBusAdministrationClient.GetQueuesAsync();

            Console.WriteLine("Listing queues...");

            await foreach (var queueProperties in queuePropertiesList)
            {
                Console.WriteLine("\t{0}", queueProperties.Name);
            }

            Console.WriteLine("Done!");
        }

        public async Task GetQueueAsync(string queueName)
        {
            var response = await _serviceBusAdministrationClient.GetQueueAsync(queueName);
            var queueProperties = response.Value;
            Console.WriteLine($"Queue description for { queueName }");
            Console.WriteLine($"    Name:                                           {queueProperties.Name}");
            Console.WriteLine($"    MasSizeinMegabytes:                             {queueProperties.MaxSizeInMegabytes}");
            Console.WriteLine($"    RequiresSessions:                               {queueProperties.RequiresSession}");
            Console.WriteLine($"    RequiresDuplicateDetection:                     {queueProperties.RequiresDuplicateDetection}");
            Console.WriteLine($"    DuplicateDetectionHistoryTimeWindow:            {queueProperties.DuplicateDetectionHistoryTimeWindow}");
            Console.WriteLine($"    LockDuration:                                   {queueProperties.LockDuration}");
            Console.WriteLine($"    DefaultMessageTimeToLive:                       {queueProperties.DefaultMessageTimeToLive}");
            Console.WriteLine($"    DeadLetteringOnMessageExpiration:               {queueProperties.DeadLetteringOnMessageExpiration}");
            Console.WriteLine($"    EnableBatchedOperations:                        {queueProperties.EnableBatchedOperations}");
            Console.WriteLine($"    MaxDeliveryCount:                               {queueProperties.MaxDeliveryCount}");
            Console.WriteLine($"    Status:                                         {queueProperties.Status}");

        }

        public async Task CreateTopicAsync(string topicName)
        {
            Console.WriteLine("Creating topic {0}...", topicName);

            var response = await _serviceBusAdministrationClient.CreateTopicAsync(topicName);
            var topicProperties = response.Value;

            Console.WriteLine("Done!");
        }

        public async Task CreateSubscriptionAsync(string topicName, string subscriptionName)
        {
            Console.WriteLine("Creating subscription {0}/subscriptions/{1}...", topicName, subscriptionName);

            var response = await _serviceBusAdministrationClient.CreateSubscriptionAsync(topicName, subscriptionName);
            var subscriptionProperties = response.Value;

            Console.WriteLine("Done!");
        }

        public async Task ListTopicsAsync()
        {
            var topicPropertiesList = _serviceBusAdministrationClient.GetTopicsAsync();

            Console.WriteLine("Listing topics...");

            await foreach (var topicProperties in topicPropertiesList)
            {
                Console.WriteLine("\t{0}", topicProperties.Name);
                await ListSubscription(topicProperties.Name);
            }

            Console.WriteLine("Done!");
        }

        public async Task ListSubscription(string topicName)
        {
            var subscriptionPropertiesList = _serviceBusAdministrationClient.GetSubscriptionsAsync(topicName);

            Console.WriteLine("Listing topics...");

            await foreach (var subscriptionProperties in subscriptionPropertiesList)
            {
                Console.WriteLine("\t\t\t{0}", subscriptionProperties.SubscriptionName);
            }

        }



        public CreateQueueOptions GetCreateQueueOption(string queueName)
        {
            return new CreateQueueOptions(queueName)
            {
                RequiresDuplicateDetection = true,
                DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(10),
                RequiresSession = true,
                MaxDeliveryCount = 20,
                DefaultMessageTimeToLive = TimeSpan.FromHours(1),
                DeadLetteringOnMessageExpiration = true
            };
        }
    }
}
