namespace StockMarket.Domain
{
    public class Trade
    {
        public long SellOrderId{get;}
        public long BuyOrderId{get;}
        public decimal Price{get;}
        public decimal Quantity { get; }

        internal Trade(long sellOrderId, long buyOrderId, decimal price, decimal quantity)
        {
            SellOrderId = sellOrderId;
            BuyOrderId = buyOrderId;
            Price = price;
            Quantity = quantity;
        }
    }
}