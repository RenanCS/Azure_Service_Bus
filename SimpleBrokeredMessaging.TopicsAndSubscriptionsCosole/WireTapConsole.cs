using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.TopicsAndSubscriptionsCosole
{
    internal class WireTapConsole
    {
        private static string _serviceBusConnectionString = "";
        private static string _ordersTopicName = "Orders";

        static async Task WriteTapConsole(string[] args)
        {
            Console.WriteLine("Wire Tap Console");
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("Press enter  to activate wire tap");
            Console.ReadLine();

            var subscriptionName = $"wiretap-{Guid.NewGuid()}";

            var administrationClient = new ServiceBusAdministrationClient(_serviceBusConnectionString);

            await administrationClient.CreateSubscriptionAsync(new CreateSubscriptionOptions(_ordersTopicName, subscriptionName)
            {
                AutoDeleteOnIdle = TimeSpan.FromMinutes(5)
            });

            var serviceBusClient = new ServiceBusClient(_serviceBusConnectionString);

            var receiver = serviceBusClient.CreateReceiver(_ordersTopicName, subscriptionName);

            Console.WriteLine($"Receiving on {subscriptionName}");
            Console.WriteLine();

            while (true)
            {
                var message = await receiver.ReceiveMessageAsync();

                if (message != null)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Received message");

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Properties");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"     ContentType            - {message.ContentType}");
                    Console.WriteLine($"     CorrelationId          - {message.CorrelationId}");
                    Console.WriteLine($"     ExpiresAt              - {message.ExpiresAt}");
                    Console.WriteLine($"     Subject                - {message.Subject}");
                    Console.WriteLine($"     MessageId              - {message.MessageId}");
                    Console.WriteLine($"     PartitionKey           - {message.PartitionKey}");
                    Console.WriteLine($"     ReplyTo                - {message.ReplyTo}");
                    Console.WriteLine($"     ReplyToSessionId       - {message.ReplyToSessionId}");
                    Console.WriteLine($"     ScheduledEnqueueTime   - {message.ScheduledEnqueueTime}");
                    Console.WriteLine($"     SessionId              - {message.SessionId}");
                    Console.WriteLine($"     TimeToLive             - {message.TimeToLive}");
                    Console.WriteLine($"     To                     - {message.To}");
                    Console.WriteLine($"     EnqueuedTime           - {message.EnqueuedTime}");
                    Console.WriteLine($"     LockedUntil            - {message.LockedUntil}");
                    Console.WriteLine($"     SequenceNumber         - {message.SequenceNumber}");
                    Console.WriteLine($"     EnqueuedTime           - {message.EnqueuedTime}");


                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("ApplicationProperties");


                    Console.ForegroundColor = ConsoleColor.White;
                    foreach (var property in message.ApplicationProperties)
                    {
                        Console.WriteLine($"     {property.Key}           - {property.Value}");
                    }

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Body");

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(message.Body.ToString());

                    Console.WriteLine();

                    await receiver.CompleteMessageAsync(message);
                }
            }
        }
    }
}
