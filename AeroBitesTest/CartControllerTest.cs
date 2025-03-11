using AeroBites.Controllers;
using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AeroBitesTest
{
    public class CartControllerTest
    {
        private readonly AeroBitesContext _context;
        private readonly CartController _controller;
        private readonly int _userId;
        private readonly int _restaurantId;
        private readonly Item _item;

        public CartControllerTest()
        {
            var options = new DbContextOptionsBuilder<AeroBitesContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new AeroBitesContext(options);
            _controller = new CartController(_context);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") };
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _userId = int.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var restaurant = new Restaurant 
            { 
                Name = "Pizzaria", 
                OwnerId = _userId 
            };

            _context.Restaurant.Add(restaurant);
            _context.SaveChanges();

            _restaurantId = _context.Restaurant.FirstOrDefault(r => r.Name == "Pizzaria").Id;

            var category = new Category
            {
                Name = "Comida",
                RestaurantId = _restaurantId
            };

            _context.Category.Add(category);
            _context.SaveChanges();

            var item = new Item 
            { 
                Name = "Burguer",
                Price = 5.99f,
                CategoryId = _context.Category.FirstOrDefault(c => c.Name == "Comida").Id
            };
            
            _context.Item.Add(item);
            _context.SaveChanges();
            _item = _context.Item.FirstOrDefault(c => c.Name == "Burguer");
        }

        [Fact]
        public async Task AddItem_ShouldAddItemToCart_WhenValidIdsAreProvided()
        {
            var result = await _controller.AddItem(_item.Id, _restaurantId);

            var cart = await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(c => c.AccountId == _userId && c.RestaurantId == _restaurantId && c.Status == 0); 
            Assert.NotNull(cart);
            Assert.Equal(_item.Name, cart.Items.First().Name);
            Assert.Equal(_userId, cart.AccountId);

            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.Equal("Menu", redirectResult.ActionName);
            Assert.Equal("Restaurant", redirectResult.ControllerName);
        }

        [Fact]
        public async Task RemoveItem_ShouldRemoveItemFromCart_WhenValidIdsAreProvided()
        {
            await this.AddItem_ShouldAddItemToCart_WhenValidIdsAreProvided();

            var cart = await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(c => c.AccountId == _userId && c.RestaurantId == _restaurantId && c.Status == 0);
            var cartFirstItemID = cart.Items.First().Id;
            var result = await _controller.RemoveItem(cartFirstItemID, _restaurantId);

            Assert.Empty(cart.Items.FindAll(i => i.Id == cartFirstItemID));

            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.Equal("Menu", redirectResult.ActionName);
            Assert.Equal("Restaurant", redirectResult.ControllerName);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
