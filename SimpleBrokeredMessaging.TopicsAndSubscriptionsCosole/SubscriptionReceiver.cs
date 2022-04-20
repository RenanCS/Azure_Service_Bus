using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBrokeredMessaging.TopicsAndSubscriptionsCosole
{
    internal class SubscriptionReceiver
    {
        private readonly SubscriptionClient _subscriptionClient;

        public SubscriptionReceiver(string connectionString, string topic, string subscriptionName)
        {
            _subscriptionClient = new SubscriptionClient(connectionString, topic, subscriptionName);

        }

        public void RegisterHandlers()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _subscriptionClient.RegisterMessageHandler(ProcessOrderMessageMessageAsync, messageHandlerOptions);
        }

        private async Task ProcessOrderMessageMessageAsync(Message message, CancellationToken token)
        {
            // Process the order message
            var orderJson = Encoding.UTF8.GetString(message.Body);
            var order = JsonConvert.DeserializeObject<Order>(orderJson);

            Console.WriteLine($"{ order.ToString() }");

            // Complete the message
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            return Task.CompletedTask;
        }



        public async Task Close()
        {
            await _subscriptionClient.CloseAsync();
        }
    }
}
