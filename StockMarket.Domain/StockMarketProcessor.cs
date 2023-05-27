namespace StockMarket.Domain
{
    public class StockMarketProcessor
    {
        private readonly List<Order> orders;
        private readonly List<Trade> trades;
        private long lastOrderId;
        private readonly PriorityQueue<Order, Order> buyOrders;
        private readonly PriorityQueue<Order, Order> sellOrders;

        public IEnumerable<Order> Orders => orders;
        public IEnumerable<Trade> Trades => trades;

        public StockMarketProcessor()
        {
            orders = new List<Order>();
            trades = new List<Trade>();
            lastOrderId = 0;
            buyOrders = new PriorityQueue<Order, Order>(new MaxComparer());
            sellOrders = new();
        }
        public long EnqueueOrder(TradeSide side, decimal price, decimal quantity)
        {
            var order = makeOrder(side, price, quantity);
            if (order.Side == TradeSide.Buy) processBuyOrder(order);
            else processSellOrder(order);
            return order.Id;
        }

        private void processSellOrder(Order order)
        {
            sellOrders.Enqueue(order, order);
            while (buyOrders.Count > 0 && buyOrders.Peek().Price >= order.Price)
            {
                makeTrade(sellOrder: order, buyOrder: buyOrders.Peek());
                buyOrders.Dequeue();
            }
        }
        private void processBuyOrder(Order order)
        {
            buyOrders.Enqueue(order, order);
            while (sellOrders.Count > 0 && sellOrders.Peek().Price <= order.Price)
            {
                makeTrade(sellOrder: sellOrders.Peek(), buyOrder: order);
                sellOrders.Dequeue();

            }
        }
        private void makeTrade(Order sellOrder, Order buyOrder)
        {
            var trade = new Trade(
                sellOrder.Id,
                buyOrder.Id,
                sellOrder.Price,
                sellOrder.Quantity);
            trades.Add(trade);
            sellOrder.DecreaseQuantity(amount: sellOrder.Quantity);
            buyOrder.DecreaseQuantity(amount: buyOrder.Quantity);
        }
        private Order makeOrder(TradeSide side, decimal price, decimal quantity)
        {
            Interlocked.Increment(ref lastOrderId);
            var order = new Order(lastOrderId, side, price, quantity);
            orders.Add(order);
            return order;
        }
    }
}