using FluentAssertions;

namespace StockMarket.Domain.Test
{
    public class StockMarketProcessorTests
    {
        [Fact]
        public void EnqueueOrder_Should_Process_Sell_Order_When_Buy_Order_Is_Already_Enqueued_Test()
        {
            //Arrange
            var sut = new StockMarketProcessor();
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
    }
}