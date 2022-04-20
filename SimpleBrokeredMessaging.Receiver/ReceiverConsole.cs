using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using SimpleBrokeredMessaging.MessageEntities;
using System;
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
        static string QueueNameGrupoSession = "demoGroupSessionqueue";

        static async Task Main(string[] args)
        {
            // Create a service bus cliente
            var client = new ServiceBusClient(ConnectionString);

            await ReceiveSimpleTextMessage(client);

            await ReceiveAndPrecessThreads(client, QueueNameObject, 1);

            await ReceiveAndPrecessThreads(client, QueueNameControl, 2);

            await ReceiveAndPrecessThreads(client, QueueNameDuplicate, 1);

            await ReceiveAndPrecessGroupOrderThreads(client, QueueNameGrupoSession);
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

        private static async Task ReceiveAndPrecessGroupOrderThreads(ServiceBusClient client, string queueName)
        {

            // Send some messages
            Console.WriteLine("Receive Group Order threads messages ... ");

            while (true)
            {      // Create a message processor
                var sessionReceiver = await client.AcceptNextSessionAsync(queueName);

                if (sessionReceiver != null)
                {
                    while (true)
                    {
                        var receivedOrderGroup = await sessionReceiver.ReceiveMessageAsync(TimeSpan.FromSeconds(1));

                        if (receivedOrderGroup != null)
                        {
                            // Deserialize the message body
                            var pizzaOrder = JsonConvert.DeserializeObject<PizzaOrder>(receivedOrderGroup.Body.ToString());

                            // Process the message
                            Console.WriteLine($"Cooking: {pizzaOrder.CustomerName},{pizzaOrder.Type},{pizzaOrder.Size}");

                            // Start the message processor
                            await sessionReceiver.CompleteMessageAsync(receivedOrderGroup);
                        }
                        else
                        {
                            break;
                        }
                    }

                    Console.WriteLine("Receiving, hit enter to exit", ConsoleColor.White);
                    Console.ReadLine();

                    // Close the message processor
                    await sessionReceiver.CloseAsync();
                }
                else
                {
                    break;
                }
            }

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
