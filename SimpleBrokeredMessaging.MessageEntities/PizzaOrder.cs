namespace SimpleBrokeredMessaging.MessageEntities
{
    public class PizzaOrder
    {
        public PizzaOrder(string customerName, string type, string size, int? id = 0)
        {
            Id = id;
            CustomerName = customerName;
            Type = type;
            Size = size;
        }

        public int? Id { get; set; }
        public string CustomerName { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
    }
}
