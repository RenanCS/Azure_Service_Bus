using System;

namespace SimpleBrokeredMessaging.TopicsAndSubscriptionsCosole
{
    internal class Order
    {
        public string Name { get; set; }
        public DateTime OrderDate { get; set; }
        public int Items { get; set; }
        public double Value { get; set; }
        public string Priority { get; set; }
        public string Region { get; set; }
        public bool HasLoyltyCard { get; set; }

        public override string ToString()
        {
            return $"{Name}\tItems:{Items}\tValue:${Value}\tRegion:{Region}\tLoyal:{HasLoyltyCard}";
        }
    }
}
