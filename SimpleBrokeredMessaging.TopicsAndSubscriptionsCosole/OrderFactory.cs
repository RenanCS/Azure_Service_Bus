using System.Collections.Generic;

namespace SimpleBrokeredMessaging.TopicsAndSubscriptionsCosole
{
    internal class OrderFactory
    {
        public static List<Order> CreateTestOrders()
        {
            var orders = new List<Order>();

            orders.Add(new Order()
            {
                Name = "Loyal Customer",
                Value = 19.99,
                Region = "USA",
                Items = 1,
                HasLoyltyCard = true
            });

            orders.Add(new Order()
            {
                Name = "Large Customer",
                Value = 49.99,
                Region = "USA",
                Items = 50,
                HasLoyltyCard = false
            });

            orders.Add(new Order()
            {
                Name = "High Customer",
                Value = 749.45,
                Region = "USA",
                Items = 45,
                HasLoyltyCard = false
            });

            orders.Add(new Order()
            {
                Name = "Loyal Europe",
                Value = 49.45,
                Region = "EU",
                Items = 3,
                HasLoyltyCard = true
            });

            orders.Add(new Order()
            {
                Name = "UK Order",
                Value = 49.45,
                Region = "UK",
                Items = 3,
                HasLoyltyCard = false
            });

            return orders;
        }
    }
}
