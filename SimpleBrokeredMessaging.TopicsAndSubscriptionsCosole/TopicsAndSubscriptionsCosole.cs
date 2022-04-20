using System;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.TopicsAndSubscriptionsCosole
{
    internal class TopicsAndSubscriptionsCosole
    {
        private static string _serviceBusConnectionString = "CONNECTION_AZURE_PORTAL";
        private static string _ordersTopicName = "Orders";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Topics and Subscriptions Console");

            PrompAndWait("Press enter to create topic and subscriptons...");
            await CreateTopicsAndSubscriptions();

            PrompAndWait("Press enter to send order messages...");
            await SendOrderMessages();

            PrompAndWait("Press enter to receive order messages...");
            await ReceiveOrdersFormAllSubscriptions();
        }

        static void PrompAndWait(string text)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(text);
            Console.ForegroundColor = temp;
            Console.ReadLine();
        }

        private static async Task ReceiveOrdersFormAllSubscriptions()
        {
            var manager = new Manager(_serviceBusConnectionString);

            // Get the subscríption from our topic...
            var subscriptions = await manager.GetSubscriptionsForTopic(_ordersTopicName);

            // Loop through the subscriptions and process subscriptions and messages.
            foreach (var subscription in subscriptions)
            {
                Console.WriteLine($"Receiving orders from {subscription}...");

                var receiver = new SubscriptionReceiver(_serviceBusConnectionString, _ordersTopicName, subscription);

                receiver.RegisterHandlers();

                Console.WriteLine("Press enter when done");

                Console.ReadLine();

                await receiver.Close();
            }
        }
        private static async Task SendOrderMessages()
        {
            var orders = OrderFactory.CreateTestOrders();

            var sender = new TopicSender(_serviceBusConnectionString, _ordersTopicName);

            foreach (var order in orders)
            {
                await sender.SendOrderMessage(order);
            }

            // Always be a good Service Bus citizan...
            await sender.Close();

        }

        static async Task CreateTopicsAndSubscriptions()
        {
            var manager = new Manager(_serviceBusConnectionString);

            await manager.CreateTopic(_ordersTopicName);
            await manager.CreateSubscription(_ordersTopicName, "AllOrders");

            await manager.CreateSubscriptionWithSqlFilter(_ordersTopicName, "UsaOrders", "region = 'USA'");
            await manager.CreateSubscriptionWithSqlFilter(_ordersTopicName, "EuOrders", "region = 'EU'");

            await manager.CreateSubscriptionWithSqlFilter(_ordersTopicName, "LargeOrders", "item > 30");
            await manager.CreateSubscriptionWithSqlFilter(_ordersTopicName, "HighValueOrders", "value > 500");

            await manager.CreateSubscriptionWithSqlFilter(_ordersTopicName, "LoyaltyCardOrders", "loyalty = true AND region = 'USA'");

            await manager.CreateSubscriptionWithCorrelationFilter(_ordersTopicName, "UkOrders", "UK");
        }
    }
}
