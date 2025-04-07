using AeroBites.Controllers;
using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static AeroBites.Enums;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;


namespace AeroBitesTest
{
    public class MyOrdersControllerTest : IDisposable
    {
        private readonly AeroBitesContext _context;
        private readonly MyOrdersController _controller;
        private readonly int _userId;

        public MyOrdersControllerTest()
        {
            var options = new DbContextOptionsBuilder<AeroBitesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AeroBitesContext(options);
            _controller = new MyOrdersController(_context);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var tempData = new TempDataDictionary(_controller.ControllerContext.HttpContext, Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;

            _userId = int.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var cart1 = new Cart { 
                Status = AeroBites.Enums.OrderStatus.Placed,
                Restaurant = "Pizzaria",
                RestaurantId = 1,
                AccountId = _userId,
                PlacedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalPrice = 19.99f
            };

            var cart2 = new Cart
            {
                Status = AeroBites.Enums.OrderStatus.Preparing,
                Restaurant = "Pizzaria",
                RestaurantId = _userId,
                AccountId = _userId,
                PlacedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalPrice = 19.99f
            };

            var cart3 = new Cart
            {
                Status = AeroBites.Enums.OrderStatus.OnTheWay,
                Restaurant = "Pizzaria",
                RestaurantId = _userId,
                AccountId = _userId,
                PlacedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalPrice = 19.99f
            };

            _context.Cart.AddRange(cart1, cart2, cart3);
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsCorrectOrders()
        {
            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Cart>>(viewResult.Model);

            Assert.Equal(3, model.Count);
            Assert.DoesNotContain(model, c => c.Status == OrderStatus.Recieved);
            Assert.All(model, c => { Assert.True(c.Status >= OrderStatus.Placed && c.Status <= OrderStatus.Waiting);});
        }

        [Fact]
        public async Task History_ReturnsOnlyReceivedOrders()
        {
            var result = await _controller.History();

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<List<Cart>>(viewResult.Model);

            if (model.Count > 0)
            {
                Assert.All(model, c => Assert.Equal(OrderStatus.Recieved, c.Status));
            }
            else
            {
                Assert.Empty(model);
            }
        }

        [Fact]
        public async Task RecieveOrder_ChangesStatusToReceived_WhenOrderIsWaiting()
        {
            var cart = new Cart
            {
                Status = OrderStatus.Waiting,
                Restaurant = "Pizzaria",
                RestaurantId = 1,
                AccountId = _userId,
                PlacedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalPrice = 19.99f
            };

            _context.Cart.Add(cart);
            await _context.SaveChangesAsync();

            var result = await _controller.RecieveOrder(cart.Id);

            var updatedOrder = await _context.Cart.FindAsync(cart.Id);
            Assert.Equal(OrderStatus.Recieved, updatedOrder.Status);
            Assert.IsType<RedirectToActionResult>(result);
            var redirectResult = (RedirectToActionResult)result;
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task RecieveOrder_DoesNotChangeStatus_WhenOrderStatusIsNotWaiting()
        {
            var cart = new Cart
            {
                Status = OrderStatus.Placed,
                Restaurant = "Pizzaria",
                RestaurantId = 1,
                AccountId = _userId,
                PlacedDate = DateOnly.FromDateTime(DateTime.UtcNow),
                TotalPrice = 19.99f
            };

            _context.Cart.Add(cart);
            await _context.SaveChangesAsync();

            var result = await _controller.RecieveOrder(cart.Id);
            var updatedOrder = await _context.Cart.FindAsync(cart.Id);

            Assert.Equal(OrderStatus.Placed, updatedOrder.Status);
            Assert.IsType<RedirectToActionResult>(result);

            var redirectResult = (RedirectToActionResult)result;
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task RecieveOrder_RedirectsToIndex_WhenOrderDoesNotExist()
        {
            var nonExistentOrderId = 999;
            var result = await _controller.RecieveOrder(nonExistentOrderId);

            Assert.IsType<RedirectToActionResult>(result);

            var redirectResult = (RedirectToActionResult)result;
            Assert.Equal("Index", redirectResult.ActionName); 
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
