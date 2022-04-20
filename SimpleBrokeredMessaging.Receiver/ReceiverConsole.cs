using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Newtonsoft.Json;
using SimpleBrokeredMessaging.MessageEntities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Receiver
{
    internal class ReceiverConsole
    {
        static string ConnectionString = "CONNECTION_AZURE_PORTAL";
        static string QueueNameText = "demotextqueue";
        static string QueueNameObject = "demoobjectqueue";
        static string QueueNameControl = "demoControlqueue";
        static string QueueNameDuplicate = "demoDuplicatequeue";

        static async Task Main(string[] args)
        {
            // Create a service bus cliente
            var client = new ServiceBusClient(ConnectionString);

            // await ReceiveSimpleTextMessage(client);

            await ReceiveAndPrecessThreads(client, QueueNameObject, 1);

            await ReceiveAndPrecessThreads(client, QueueNameControl, 2);

            await ReceiveAndPrecessThreads(client, QueueNameDuplicate, 1);

        }

        private static async Task ReceiveSimpleTextMessage(ServiceBusClient client)
        {
            // Create a service bus receiver  
            var receiver = client.CreateReceiver(QueueNameText);

            // Send some messages
            Console.WriteLine("Receive messages ... ");

            var message = await receiver.ReceiveMessageAsync();
            if (message != null)
            {
                Console.Write(message.Body.ToString());

                // Complete the message
                await receiver.CompleteMessageAsync(message);
            }

            Console.WriteLine();
            Console.WriteLine("All messages received");

            // Close the receiver
            await receiver.CloseAsync();

            Console.WriteLine("Finally messages.");
        }

        private static async Task ReceiveAndPrecessThreads(ServiceBusClient client, string queueName, int threads)
        {

            // Send some messages
            Console.WriteLine("Receive threads messages ... ");

            // Set the options for processing messages
            var options = new ServiceBusProcessorOptions()
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = threads,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10)
            };

            // Create a message processor
            var processor = client.CreateProcessor(queueName, options);

            // Add habndler to process and error message
            processor.ProcessMessageAsync += ProcessPizzaMessageAsync;
            processor.ProcessErrorAsync += ErrorHandler;

            // Start the message processor
            await processor.StartProcessingAsync();

            Console.WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
            Console.ReadLine();

            // Stop and close the message processor
            await processor.StopProcessingAsync();
            await processor.CloseAsync();
        }



        static async Task ProcessPizzaMessageAsync(ProcessMessageEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.Message.Body.ToString()))
            {
                // Deserialize the message body
                var pizzaOrder = JsonConvert.DeserializeObject<PizzaOrder>(args.Message.Body.ToString());

                // Process the message
                Console.WriteLine($"Cooking: {pizzaOrder.CustomerName},{pizzaOrder.Type},{pizzaOrder.Size}");
            }

            // Complete the message receive operation
            await args.CompleteMessageAsync(args.Message);
        }

        static async Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.Message, ConsoleColor.Red);
        }


    }
}
