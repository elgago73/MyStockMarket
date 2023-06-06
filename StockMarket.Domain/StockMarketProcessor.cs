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
            sellOrders = new PriorityQueue<Order, Order>(new MinComparer());
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
            while (buyOrders.Count > 0 && buyOrders.Peek().Price >= order.Price && order.Quantity > 0)
            {
                var peekedBuyOrder = buyOrders.Peek();
                makeTrade(sellOrder: order, buyOrder: peekedBuyOrder);
                if (peekedBuyOrder.Quantity == 0) buyOrders.Dequeue();
            }
            if (order.Quantity > 0)
            {
                sellOrders.Enqueue(order, order);
            }
        }
        private void processBuyOrder(Order order)
        {
            while (sellOrders.Count > 0 && sellOrders.Peek().Price <= order.Price && order.Quantity > 0)
            {
                Order peekedSellOrder = sellOrders.Peek();
                makeTrade(sellOrder: peekedSellOrder, buyOrder: order);
                if (peekedSellOrder.Quantity == 0) sellOrders.Dequeue();


            }
            if (order.Quantity > 0)
            {
                buyOrders.Enqueue(order, order);
            }
        }
        private void makeTrade(Order sellOrder, Order buyOrder)
        {
            var quantity = Math.Min(sellOrder.Quantity, buyOrder.Quantity);
            var trade = new Trade(
                sellOrder.Id,
                buyOrder.Id,
                sellOrder.Price,
                quantity);
            trades.Add(trade);
            sellOrder.DecreaseQuantity(amount: quantity);
            buyOrder.DecreaseQuantity(amount: quantity);
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