using StockMarket.Domain.Comparers;
using System.Security.Cryptography.X509Certificates;

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

            if (order.Side == TradeSide.Buy)
            {
                matchOrder(order: order,
                    orders: buyOrders,
                    matchingOrders: sellOrders,
                    comparePriceDelegate: (decimal price1, decimal price2) => price1 <= price2);
            }
            else
            {
                matchOrder(order: order,
                    orders: sellOrders,
                    matchingOrders: buyOrders,
                    comparePriceDelegate: (decimal price1, decimal price2) => price1 >= price2);
            }

            return order.Id;
        }

        private void matchOrder(Order order,
                                     PriorityQueue<Order, Order> orders,
                                     PriorityQueue<Order, Order> matchingOrders,
                                     Func<decimal, decimal, bool> comparePriceDelegate)
        {
            while(order.Quantity > 0 && matchingOrders.Count > 0 && comparePriceDelegate(matchingOrders.Peek().Price, order.Price))
            {
                var peekedMatchingOrder = matchingOrders.Peek();
                makeTrade(peekedMatchingOrder, order);
                if (peekedMatchingOrder.Quantity == 0) matchingOrders.Dequeue();

            }

            if (order.Quantity > 0) orders.Enqueue(order, order);
        }

        private void makeTrade(Order order1, Order order2)
        {
            (Order sellOrder, Order BuyOrder) = findOrders(order1, order2);
            var quantity = Math.Min(sellOrder.Quantity, BuyOrder.Quantity);
           
            var trade = new Trade(
                sellOrder.Id,
                BuyOrder.Id,
                sellOrder.Price,
                quantity);
            trades.Add(trade);
            
            sellOrder.DecreaseQuantity(amount: quantity);
            BuyOrder.DecreaseQuantity(amount: quantity);
        }

        private static (Order sellOrder, Order BuyOrder) findOrders(Order order1, Order order2)
        {
            if(order1.Side == TradeSide.Sell) return (sellOrder: order1, BuyOrder: order2);
            return (sellOrder: order2, BuyOrder: order1);
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