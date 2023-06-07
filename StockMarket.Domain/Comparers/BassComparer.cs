namespace StockMarket.Domain.Comparers
{
    internal abstract class BassComparer : IComparer<Order>
    {
        public int Compare(Order? x, Order? y)
        {
            var result = SpecificCompare(x, y);
            if (result != 0) return result;

            if (x?.Id < y?.Id) return -1;
            if (x?.Id > y?.Id) return 1;

            return 0;
        }

        protected abstract int SpecificCompare(Order? x, Order? y);
    }
}
