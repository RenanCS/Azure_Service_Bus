using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Sender
{
    public class SenderConsole
    {
        static string ConnectionString = "CONNECTION_AZURE_PORTAL";
        static string QueueName = "demoqueue";
        static string Sentence = "Microsoft Azure Service Bus.";

        static async Task Main(string[] args)
        {
            // Create a service bus cliente
            var client = new ServiceBusClient(ConnectionString);

            // Create a service bus sender  
            var sender = client.CreateSender(QueueName);

            // Send some messages
            Console.WriteLine("Send messages ... ");
            foreach (var character in Sentence)
            {

                var message = new ServiceBusMessage(character.ToString());
                await sender.SendMessageAsync(message);
                Console.WriteLine($" Sent: { character }");
            }

            // Close the sender
            await sender.CloseAsync();

            Console.WriteLine("Sent messages.");
            Console.ReadLine();

        }
    }
}
