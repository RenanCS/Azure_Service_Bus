using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.TopicsAndSubscriptionsCosole
{
    internal class Manager
    {
        private ServiceBusAdministrationClient _serviceBusAdministrationClient;

        public Manager(string connectionString)
        {
            _serviceBusAdministrationClient = new ServiceBusAdministrationClient(connectionString);
        }

        public async Task<TopicProperties> CreateTopic(string topicName)
        {
            Console.WriteLine($"Creating Topic {topicName}");

            if (await _serviceBusAdministrationClient.TopicExistsAsync(topicName))
            {
                await _serviceBusAdministrationClient.DeleteTopicAsync(topicName);
            }

            return await _serviceBusAdministrationClient.CreateTopicAsync(topicName);
        }

        public async Task<SubscriptionProperties> CreateSubscription(string topicName, string subscriptionName)
        {
            Console.WriteLine($"Creating subscription  {topicName}/{subscriptionName}");

            return await _serviceBusAdministrationClient.CreateSubscriptionAsync(topicName, subscriptionName);
        }

        internal async Task<IList<string>> GetSubscriptionsForTopic(string ordersTopicName)
        {
            var subscriptionPropertiesList = _serviceBusAdministrationClient.GetSubscriptionsAsync(ordersTopicName);

            Console.WriteLine("Listing topics...");

            var listSubscriptions = new List<string>();

            await foreach (var subscriptionProperties in subscriptionPropertiesList)
            {
                listSubscriptions.Add(subscriptionProperties.SubscriptionName);
            }

            return listSubscriptions;
        }

        public async Task<SubscriptionProperties> CreateSubscriptionWithSqlFilter(string topicName, string subscriptionName, string sqlExpressions)
        {
            Console.WriteLine($"Creating subscription with SQL Filter   {topicName}/{subscriptionName}/({sqlExpressions})");

            var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName);

            var ruleOptions = new CreateRuleOptions("Default", new SqlRuleFilter(sqlExpressions));

            return await _serviceBusAdministrationClient.CreateSubscriptionAsync(subscriptionOptions, ruleOptions);
        }

        public async Task<SubscriptionProperties> CreateSubscriptionWithCorrelationFilter(string topicName, string subscriptionName, string correlationId)
        {
            Console.WriteLine($"Creating subscription with correlation filter  {topicName}/{subscriptionName}/({correlationId})");

            var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName);

            var ruleOptions = new CreateRuleOptions("Default", new CorrelationRuleFilter(correlationId));

            return await _serviceBusAdministrationClient.CreateSubscriptionAsync(subscriptionOptions, ruleOptions);
        }
    }
}
