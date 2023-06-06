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


        [Fact]
        public void EnqueueOrder_Should_Process_Buy_Order_When_Multiple_Sell_Orders_Are_Already_Enqueued_Test()
        {
            //Arrange
            var buyOrderId1 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 1);
            var buyOrderId2 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 1);
            //Act
            var sellOrderId = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 2);
            //Assert
            Assert.Equal(3, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1500M,
                Quantity = 0M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1500M,
                Quantity = 0M
            });
            sut.Orders.Skip(2).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 0M
            });
            Assert.Equal(2, sut.Trades.Count());
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId1,
                SellOrderId = sellOrderId,
                Price = 1500M,
                Quantity = 1M
            });
            sut.Trades.Skip(1).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId2,
                SellOrderId = sellOrderId,
                Price = 1500M,
                Quantity = 1M
            });
        }


        [Fact]
        public void EnqueueOrder_Should_Process_Buy_Order_When_Multiple_Sell_Orders_With_different_prices_Are_Already_Enqueued_Test()
        {
            //Arrange
            var buyOrderId1 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 1);
            var buyOrderId2 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1600M, quantity: 1);
            //Act
            var sellOrderId = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 2);
            //Assert
            Assert.Equal(3, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1500M,
                Quantity = 0M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1600M,
                Quantity = 0M
            });
            sut.Orders.Skip(2).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 0M
            });
            Assert.Equal(2, sut.Trades.Count());
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId2,
                SellOrderId = sellOrderId,
                Price = 1500M,
                Quantity = 1M
            });
            sut.Trades.Skip(1).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId1,
                SellOrderId = sellOrderId,
                Price = 1500M,
                Quantity = 1M
            });
        }


        [Fact]
        public void EnqueueOrder_Should_Process_Sell_Order_When_Multiple_Buy_Orders_Are_Already_Enqueued_Test()
        {
            //Arrange
            var sellOrderId1 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 1M);
            var sellOrderId2 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 1M);
            //Act
            var buyOrderId = sut.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 2M);
            //Assert
            Assert.Equal(3, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 0M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 0M
            });
            sut.Orders.Skip(2).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1500M,
                Quantity = 0M
            });
            Assert.Equal(2, sut.Trades.Count());
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId1,
                Price = 1500M,
                Quantity = 1M
            });
            sut.Trades.Skip(1).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId2,
                Price = 1500M,
                Quantity = 1M
            });
        }


        [Fact]
        public void EnqueueOrder_Should_Process_Sell_Order_When_Multiple_Buy_Orders_With_different_prices_Are_Already_Enqueued_Test()
        {
            //Arrange
            var sellOrderId1 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 1M);
            var sellOrderId2 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1400M, quantity: 1M);
            //Act
            var buyOrderId = sut.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 2M);
            //Assert
            Assert.Equal(3, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 0M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1400M,
                Quantity = 0M
            });
            sut.Orders.Skip(2).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1500M,
                Quantity = 0M
            });
            Assert.Equal(2, sut.Trades.Count());
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId2,
                Price = 1400M,
                Quantity = 1M
            });
            sut.Trades.Skip(1).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId,
                SellOrderId = sellOrderId1,
                Price = 1500M,
                Quantity = 1M
            });
        }


        [Fact]
        public void EnqueueOrder_Should_Process_Sell_Order_When_Multiple_Buy_and_Sell_Orders_have_been_Enqueued_Test()
        {
            //Arrange
            var buyOrderId1 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1400M, quantity: 1);
            var buyOrderId2 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1400M, quantity: 1);

            var sellOrderId1 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1400M, quantity: 1);
            //Act
            var sellOrderId2 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1400M, quantity: 1);
            //Assert
            Assert.Equal(4, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1400M,
                Quantity = 0M 
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1400M,
                Quantity = 0M 
            });
            sut.Orders.Skip(2).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1400M,
                Quantity = 0M 
            });
            sut.Orders.Skip(3).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1400M,
                Quantity = 0M 
            });
            Assert.Equal(2, sut.Trades.Count());
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId1,
                SellOrderId = sellOrderId1,
                Price = 1400M,
                Quantity = 1M
            });
            sut.Trades.Skip(1).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId2,
                SellOrderId = sellOrderId2,
                Price = 1400M,
                Quantity = 1M
            });
        }


        [Fact]
        public void EnqueueOrder_Should_Process_Buy_Order_When_Multiple_Buy_and_Sell_Orders_have_been_Enqueued_Test()
        {
            //Arrange
            var buyOrderId1 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1800M, quantity: 1);

            var sellOrderId1 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 1);
            var sellOrderId2 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1600M, quantity: 1);
            //Act
            var buyOrderId2 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1700M, quantity: 1);
            //Assert
            Assert.Equal(4, sut.Orders.Count());
            sut.Orders.First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1800M,
                Quantity = 0M
            });
            sut.Orders.Skip(1).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 0M
            });
            sut.Orders.Skip(2).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1600M,
                Quantity = 0M
            });
            sut.Orders.Skip(3).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1700M,
                Quantity = 0M
            });
            Assert.Equal(2, sut.Trades.Count());
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId1,
                SellOrderId = sellOrderId1,
                Price = 1500M,
                Quantity = 1M
            });
            sut.Trades.Skip(1).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId2,
                SellOrderId = sellOrderId2,
                Price = 1600M,
                Quantity = 1M
            });
        }


        [Fact]
        public void EnqueueOrder_Should_Process_Buy_And_Sell_Orders_When_Multiple_Buy_and_Sell_Orders_have_been_Enqueued_Test()
        {
            //Arrange
            var buyOrderId1 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1400M, quantity: 1);
            var buyOrderId2 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1500M, quantity: 3);
            var buyOrderId3 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1700M, quantity: 2);

            var sellOrderId1 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1300M, quantity: 1);
            var sellOrderId2 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1500M, quantity: 3);
            //Act
            var buyOrderId4 = sut.EnqueueOrder(side: TradeSide.Buy, price: 1700M, quantity: 1);
            var sellOrderId3 = sut.EnqueueOrder(side: TradeSide.Sell, price: 1600M, quantity: 3);
            //Assert
            Assert.Equal(7, sut.Orders.Count());
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
            sut.Orders.Skip(2).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1700M,
                Quantity = 0M
            });
            sut.Orders.Skip(3).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1300M,
                Quantity = 0M
            });
            sut.Orders.Skip(4).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1500M,
                Quantity = 0M
            });
            sut.Orders.Skip(5).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Buy,
                Price = 1700M,
                Quantity = 0M
            });
            sut.Orders.Skip(6).First().Should().BeEquivalentTo(new
            {
                Side = TradeSide.Sell,
                Price = 1600M,
                Quantity = 2M
            });
            Assert.Equal(4, sut.Trades.Count());
            sut.Trades.First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId3,
                SellOrderId = sellOrderId1,
                Price = 1300M,
                Quantity = 1M
            });
            sut.Trades.Skip(1).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId3,
                SellOrderId = sellOrderId2,
                Price = 1500M,
                Quantity = 1M
            });
            sut.Trades.Skip(2).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId2,
                SellOrderId = sellOrderId2,
                Price = 1500M,
                Quantity = 2M
            });
            sut.Trades.Skip(3).First().Should().BeEquivalentTo(new
            {
                BuyOrderId = buyOrderId4,
                SellOrderId = sellOrderId3,
                Price = 1600M,
                Quantity = 1M
            });
        }

    }
}