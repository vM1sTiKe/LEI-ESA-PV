using AeroBites;
using AeroBites.Controllers;
using AeroBites.Data;
using AeroBites.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using AeroBites.Models;

namespace AeroBitesTest
{
    public class AddressControllerTest : IDisposable
    {
        private readonly AeroBitesContext _context;
        private readonly AddressService _service;
        private readonly AddressController _controller;
        private readonly int _userId;

        public AddressControllerTest()
        {
            var options = new DbContextOptionsBuilder<AeroBitesContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AeroBitesContext(options);
            _service = new AddressService(_context);
            _controller = new AddressController(_context, _service);

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
        }

        [Fact]
        public async Task AddAddress_AddsNewAddressAndRedirects()
        {
            var result = await _controller.AddAddress(40.00, -8.0, "Nova morada");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Single(_context.Address);
        }

        [Fact]
        public async Task RemoveAddress_RemovesAddressAndRedirects()
        {
            await _controller.AddAddress(40.00, -8.0, "Nova morada");
            var address = _context.Address.First();
            var result = await _controller.RemoveAddress(address.Id);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Empty(_context.Address);
        }

        [Fact]
        public async Task SelectAddress_ActivatesAddress()
        {
            await _controller.AddAddress(40.00, -8.0, "Nova morada 1");
            await _controller.AddAddress(50.00, -98.0, "Nova morada 2");

            var allAddresses = _context.Address.OrderBy(a => a.Id).ToList();

            var address1 = allAddresses[0];
            var address2 = allAddresses[1];

            var result = await _controller.SelectAddress(address2.Id);

            Assert.IsType<OkResult>(result);

            var updated1 = await _context.Address.FindAsync(address1.Id);
            var updated2 = await _context.Address.FindAsync(address2.Id);

            Assert.False(updated1.IsActive);
            Assert.True(updated2.IsActive);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
