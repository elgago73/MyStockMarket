namespace StockMarket.Domain
{
    internal class MinComparer : IComparer<Order>
    {
        public int Compare(Order? x, Order? y)
        {
            if (x?.Price < y?.Price) return -1;
            if (x?.Price > y?.Price) return 1;
            if (x?.Id < y?.Id) return -1;
            if (x?.Id > y?.Id) return 1;
            return 0;
        }
    }
}