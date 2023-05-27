using FluentAssertions;

namespace StockMarket.Domain.Test
{
    public class StockMarketProcessorTests
    {
        private readonly StockMarketProcessor sut;
        public StockMarketProcessorTests()
        {
            sut = new StockMarketProcessor();
        }
        [Fact]
        public void EnqueueOrder_Should_Process_Sell_Order_When_Buy_Order_Is_Already_Enqueued_Test()
        {
            //Arrange
            var buyOrderId = sut.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 1M);
            //Act
            var sellOrderId = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 1M);
            //Assert
            Assert.Equal(2, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1500M,
                Quantity = 0M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 0M

            });
            Assert.Single(sut.Trades);
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId,
                Price = 1500M,
                Quantity = 1M
            });
        }
        [Fact]
        public void EnqueueOrder_Should_Process_Buy_Order_When_Sell_Order_Is_Already_Enqueued_Test()
        {
            //Arrange 
            var sellOrderId = sut.EnqueueOrder(side: TradeSide.Sell, price: 1400M, quantity: 2M);
            //Act
            var buyOrderId = sut.EnqueueOrder(side: TradeSide.Buy, price: 1400M, quantity: 2M);
            //Assert
            Assert.Equal(2, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1400M,
                Quantity = 0M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1400M,
                Quantity = 0M
            });
            Assert.Single(sut.Trades);
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId,
                Price = 1400M,
                Quantity = 2M
            });
        }
        [Fact]
        public void EnqueueOrder_Should_Not_Process_Buy_Order_When_Sell_Order_Is_Not_Match_Test()
        {
            //Arrange
            var buyOrderId = sut.EnqueueOrder(side: TradeSide.Buy, price: 1400, quantity: 1);
            //Act
            var sellOrderId = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500, quantity: 1);
            //Assert
            Assert.Equal(2, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1400M,
                Quantity = 1M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 1M
            });
            Assert.Empty(sut.Trades);
        }
        [Fact]
        public void EnqueueOrder_Should_Not_Process_Sell_Order_When_Buy_Order_Is_Not_Match_Test()
        {
            //Arrange
            var sellOrderId = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 1M);
            //Act
            var buyOrderId = sut.EnqueueOrder(side: TradeSide.Buy, price: 1400M, quantity: 1M);
            //Assert
            Assert.Equal(2, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 1M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1400M,
                Quantity = 1M
            });
            Assert.Empty(sut.Trades);
        }
        [Fact]
        public void EnqueueOrder_Should_Not_Process_Sell_Order_When_No_Buy_Order_Has_Been_Enqueued()
        {
            //Arrange
            var sut = new StockMarketProcessor();
            var buyOrderId1 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1400M, quantity: 1M);
            //Act
            var buyOrderId2 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 1M);
            //Assert
            Assert.Equal(2, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1400M,
                Quantity = 1M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1500M,
                Quantity = 1M
            });
            Assert.Empty(sut.Trades);
        }
        //[Fact]
        //public void EnqueueOrder_Should_Process_Sell_Order_When_Multiple_Buy_Orders_Are_Already_Enqueued_Test()
        //{
        //    //Arrange
        //    var buyOrderId1 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 2);
        //    var buyOrderId2 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1400M, quantity: 2);
        //    //Act
        //    var sellOrderId1 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 2);
        //    //Assert
        //}
    }
}