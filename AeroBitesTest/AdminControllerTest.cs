using AeroBites;
using AeroBites.Controllers;
using AeroBites.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AeroBitesTest
{
    public class AdminControllerTest
    {
        private readonly AeroBitesContext _context;
        private readonly AdminController _controller;

        public AdminControllerTest() 
        {
            var options = new DbContextOptionsBuilder<AeroBitesContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new AeroBitesContext(options);
            _controller = new AdminController(_context);

            var httpContext = new DefaultHttpContext();

            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;
        }

        [Fact]
        public async Task ApproveRestaurant_ShouldApproveRestaurant()
        {
            await new MyRestaurantControllerTests().Create_ShouldCreateRestaurant_WhenValidNameIsProvided();
            var restaurants = _context.Restaurant.ToList().FindAll(r => r.Status == 0);

            Assert.NotEmpty(restaurants);

            var result = _controller.ApproveRestaurant(restaurants.First().Id);
            var search = _context.Restaurant.First(r => r.Id == restaurants.First().Id);

            Assert.Equal(Enums.RestaurantStatus.Valid, search.Status);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
