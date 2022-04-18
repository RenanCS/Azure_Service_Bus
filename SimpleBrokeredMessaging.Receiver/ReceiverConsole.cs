using Azure.Messaging.ServiceBus;
using System;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Receiver
{
    internal class ReceiverConsole
    {
        static string ConnectionString = "CONNECTION_AZURE_PORTAL";
        static string QueueName = "demoqueue";

        static async Task Main(string[] args)
        {
            // Create a service bus cliente
            var client = new ServiceBusClient(ConnectionString);

            // Create a service bus receiver  
            var receiver = client.CreateReceiver(QueueName);

            // Send some messages
            Console.WriteLine("Receive messages ... ");

            while (true)
            {
                var message = await receiver.ReceiveMessageAsync();
                if (message != null)
                {
                    Console.Write(message.Body.ToString());

                    // Complete the message
                    await receiver.CompleteMessageAsync(message);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("All messages received");
                    break;
                }
            }

            // Close the receiver
            await receiver.CloseAsync();

            Console.WriteLine("Finally messages.");
            Console.ReadLine();

        }
    }
}
