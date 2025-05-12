namespace BusLogic.Requests
{
    public class RequestTransaction
    {

        public required string TransactionIdCC { get; set; }

        public decimal Count { get; set; }

        public required string RequestProperty { get; set; }
    }
}
