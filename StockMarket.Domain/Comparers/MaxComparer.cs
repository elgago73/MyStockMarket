using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.Domain.Comparers
{
    internal class MaxComparer : BassComparer
    {
        protected override int SpecificCompare(Order? x, Order? y)
        {
            if (x?.Price < y?.Price) return 1;
            if (x?.Price > y?.Price) return -1;

            return 0;
        }
    }
}
