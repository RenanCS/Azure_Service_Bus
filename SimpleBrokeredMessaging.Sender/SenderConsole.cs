using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using SimpleBrokeredMessaging.MessageEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.Sender
{
    public class SenderConsole
    {
        static string ConnectionString = "CONNECTION_AZURE_PORTAL";
        static string QueueNameText = "demotextqueue";
        static string QueueNameObject = "demoobjectqueue";
        static string QueueNameControl = "demoControlqueue";
        static string QueueNameDuplicate = "demoDuplicatequeue";
        static string QueueNameGrupoSession = "demoGroupSessionqueue";
        static string Sentence = "Microsoft Azure Service Bus.";

        static async Task Main(string[] args)
        {
            // Create a service bus cliente
            var client = new ServiceBusClient(ConnectionString);

            await SendSimpleTextMessage(client);

            await SendSimpleObjectMessage(client);

            await SendSimpleControlOrderMessage(client);

            await SendSimpleOrderBatchPizza(client);

            await SendSimpleDuplicateOrderBatchPizza(client);

            await SendSimpleSessionGroupOrderBatchPizza(client);

        }

        private static async Task SendSimpleTextMessage(ServiceBusClient client)
        {
            // Create a service bus sender  
            var sender = client.CreateSender(QueueNameText);

            // Send some messages
            Console.WriteLine("Send messages ... ");

            var message = new ServiceBusMessage(Sentence.ToString());
            await sender.SendMessageAsync(message);
            Console.WriteLine($" Sent: { Sentence }");

            // Close the sender
            await sender.CloseAsync();

            Console.WriteLine("Sent messages.");

        }

        private static async Task SendSimpleObjectMessage(ServiceBusClient client)
        {

            var order = new PizzaOrder("Renan Carvalho de Souza", "Hawaiian", "Large");

            // Serialize the order object 
            var jasonPizzaOrder = JsonConvert.SerializeObject(order);

            // Create a Message
            var message = new ServiceBusMessage(jasonPizzaOrder)
            {
                Subject = "PizzaOrder",
                ContentType = "application/json",
            };


            // Create a service bus sender  
            var sender = client.CreateSender(QueueNameObject);

            Console.WriteLine($" Send order ....", ConsoleColor.Green);

            // Send the message
            await sender.SendMessageAsync(message);

            // Close the sender
            await sender.CloseAsync();

            Console.WriteLine("Finally send order.");

        }

        private static async Task SendSimpleControlOrderMessage(ServiceBusClient client)
        {
            Console.WriteLine($" SendSimpleControlOrderMessage....", ConsoleColor.Cyan);

            // Create a Message
            var message = new ServiceBusMessage()
            {
                Subject = "ControlOrder",
            };

            // Add some application properties tto the property collection
            message.ApplicationProperties.Add("SystemId", 1234);
            message.ApplicationProperties.Add("Command", "Pending Restart");
            message.ApplicationProperties.Add("ActionTime", DateTime.UtcNow.AddHours(2));


            // Create a service bus sender  
            var sender = client.CreateSender(QueueNameControl);

            Console.WriteLine($" Send control order ....", ConsoleColor.Green);

            // Send the message
            await sender.SendMessageAsync(message);

            // Close the sender
            await sender.CloseAsync();

            Console.WriteLine("Finally control order.");

        }

        private static async Task SendSimpleOrderBatchPizza(ServiceBusClient client)
        {
            Console.WriteLine($" SendSimpleOrderBatchPyzza....", ConsoleColor.Cyan);

            var pizzaOrderList = new List<PizzaOrder>()
            {
                new PizzaOrder("Renan Carvalho de Souza 1", "Hawaiian", "Large"),
                new PizzaOrder("Renan Carvalho de Souza 2", "Hawaiian", "Large"),
                new PizzaOrder("Renan Carvalho de Souza 3", "Hawaiian", "Large"),
            };

            // Create a service bus sender  
            var sender = client.CreateSender(QueueNameControl);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            foreach (var pizza in pizzaOrderList)
            {
                // Serialize the order object 
                var jasonPizza = JsonConvert.SerializeObject(pizza);

                // Create a Message
                var message = new ServiceBusMessage(jasonPizza)
                {
                    Subject = "PizzaOrder",
                    ContentType = "application/json",
                };

                if (!messageBatch.TryAddMessage(message))
                {
                    throw new Exception("The message is too large to fit in the batch");
                }

            }

            Console.WriteLine($" Send batch order ....", ConsoleColor.Green);

            // Send the message
            await sender.SendMessagesAsync(messageBatch);

            // Close the sender
            await sender.CloseAsync();

            Console.WriteLine("Finally batch order.");
        }

        private static async Task SendSimpleDuplicateOrderBatchPizza(ServiceBusClient client)
        {
            Console.WriteLine($" SendSimpleDuplicateOrderBatchPyzza....", ConsoleColor.Cyan);

            var pizzaOrderList = new List<PizzaOrder>()
            {
                new PizzaOrder("Renan Carvalho de Souza 1", "Hawaiian", "Large", 55),
                new PizzaOrder("Renan Carvalho de Souza 1", "Hawaiian", "Large", 55),
                new PizzaOrder("Renan Carvalho de Souza 1", "Hawaiian", "Large", 55),
            };

            // Create a service bus sender  
            var sender = client.CreateSender(QueueNameDuplicate);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            foreach (var pizza in pizzaOrderList)
            {
                // Serialize the order object 
                var jasonPizza = JsonConvert.SerializeObject(pizza);

                // Create a Message
                var message = new ServiceBusMessage(jasonPizza)
                {
                    Subject = "PizzaOrder",
                    ContentType = "application/json",
                    MessageId = pizza.Id.ToString(),
                };

                if (!messageBatch.TryAddMessage(message))
                {
                    throw new Exception("The message is too large to fit in the batch");
                }

            }

            Console.WriteLine($" Send batch order ....", ConsoleColor.Green);

            // Send the message
            await sender.SendMessagesAsync(messageBatch);

            // Close the sender
            await sender.CloseAsync();

            Console.WriteLine("Finally batch order.");
        }



        private static async Task SendSimpleSessionGroupOrderBatchPizza(ServiceBusClient client)
        {
            Console.WriteLine($" SendSimpleDuplicateOrderBatchPyzza....", ConsoleColor.Cyan);

            var pizzaOrderList = new List<PizzaOrder>()
            {
                new PizzaOrder("Renan Carvalho de Souza 1", "Hawaiian", "Large", 55),
                new PizzaOrder("Renan Carvalho de Souza 2", "Hawaiian", "Large", 55),
                new PizzaOrder("Renan Carvalho de Souza 3", "Hawaiian", "Large", 55),

                 new PizzaOrder("Renan Carvalho de Souza 1", "Hawaiian", "Small", 56),
                new PizzaOrder("Renan Carvalho de Souza 2", "Hawaiian", "Small", 56),
                new PizzaOrder("Renan Carvalho de Souza 3", "Hawaiian", "Small", 56),
            };

            // Create a service bus sender  
            var sender = client.CreateSender(QueueNameGrupoSession);

            using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

            foreach (var pizza in pizzaOrderList)
            {
                // Serialize the order object 
                var jasonPizza = JsonConvert.SerializeObject(pizza);

                // Create a Message
                var message = new ServiceBusMessage(jasonPizza)
                {
                    Subject = "PizzaOrder",
                    ContentType = "application/json",
                    SessionId = pizza.Id.ToString(),
                };

                if (!messageBatch.TryAddMessage(message))
                {
                    throw new Exception("The message is too large to fit in the batch");
                }

            }

            Console.WriteLine($" Send batch order ....", ConsoleColor.Green);

            // Send the message
            await sender.SendMessagesAsync(messageBatch);

            // Close the sender
            await sender.CloseAsync();

            Console.WriteLine("Finally batch order.");
        }




    }
}
