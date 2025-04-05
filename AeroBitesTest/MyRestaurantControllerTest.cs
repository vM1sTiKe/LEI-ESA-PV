using AeroBites.Controllers;
using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace AeroBitesTest
{
    public class MyRestaurantControllerTests
    {
        private readonly AeroBitesContext _context;
        private readonly MyRestaurantController _controller;
        private readonly int _ownerId;

        public MyRestaurantControllerTests()
        {
            // Configura��o inicial
            var options = new DbContextOptionsBuilder<AeroBitesContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;

            _context = new AeroBitesContext(options);
            _controller = new MyRestaurantController(_context, null);

            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "1") }; // Criar o utilizador falso com um ID fixo
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _ownerId = int.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        }

        [Fact]
        public async Task Create_ShouldCreateRestaurant_WhenValidNameIsProvided()
        {
            var restaurant = new Restaurant
            {
                Name = "Test Restaurant",
                OwnerId = _ownerId // Usa o OwnerId configurado no setup
            };

            var result = await _controller.Create(restaurant);

            var createdRestaurant = await _context.Restaurant.FirstOrDefaultAsync(r => r.Name == "Test Restaurant");
            Assert.NotNull(createdRestaurant); // Verifica se o restaurante foi criado
            Assert.Equal("Test Restaurant", createdRestaurant.Name); // Verifica se o nome do restaurante � correto

            // Verifica se o resultado da a��o foi uma redire��o
            var redirectResult = result as RedirectToActionResult;
            Assert.NotNull(redirectResult);
            Assert.Equal("Reviewing", redirectResult.ActionName); // Verifica se a a��o foi redirecionada para "Reviewing"
        }

        // Limpa a bd ap�s cada teste
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}