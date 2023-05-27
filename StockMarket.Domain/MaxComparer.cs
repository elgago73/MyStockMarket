using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockMarket.Domain
{
    internal class MaxComparer : IComparer<Order>
    {
        public int Compare(Order? x, Order? y)
        {
            if(x?.Price < y?.Price) return -1;
            if(x?.Price > y?.Price) return 1;
            if(x?.Id < y?.Id) return -1;
            if(x?.Id > y?.Id) return 1;
            return 0;
        }
    }
}
