namespace StockMarket.Domain.Comparers
{
    internal class MinComparer : BassComparer
    {

        protected override int SpecificCompare(Order? x, Order? y)
        {
            if (x?.Price < y?.Price) return -1;
            if (x?.Price > y?.Price) return 1;

            return 0;
        }
    }
}