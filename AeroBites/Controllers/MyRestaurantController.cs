using AeroBites.Data;
using AeroBites.Models;
using AeroBites.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AeroBites.Controllers
{
    [Authorize]
    public class MyRestaurantController(AeroBitesContext context, AddressService addressService) : Controller
    {
        private Restaurant? MyRestaurant => context.Restaurant.Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefault(r => r.OwnerId == User.GetId());
        private IQueryable<Cart>? MyOrders => context.Cart.Where(c => MyRestaurant != null && c.RestaurantId == MyRestaurant.Id);


        /// <summary>
        /// Displays the restaurant's home page or redirects to the create page if no restaurant exists.
        /// Redirects to the Reviewing page if the restaurant is still waiting for acceptance.
        /// </summary>
        /// <returns>
        /// The restaurant's homepage view or the create page.
        /// </returns>
        public IActionResult Index() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Create));
            if (MyRestaurant.Status == Enums.RestaurantStatus.WaitingAcceptance) return RedirectToAction(nameof(Reviewing));
            return View(MyRestaurant);
        }

        /// <summary>
        /// Displays the restaurant creation page if no restaurant exists.
        /// Redirects to the index if a restaurant is already created.
        /// </summary>
        /// <returns>
        /// The restaurant creation view or redirect to the index.
        /// </returns>
        public IActionResult Create() {
            return MyRestaurant is null ? View() : RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Handles the creation of a new restaurant.
        /// </summary>
        /// <param name="restaurant">The new restaurant data to be created.</param>
        /// <returns>
        /// A redirect to the Reviewing page after creation.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")] Restaurant restaurant) {
            restaurant.OwnerId = User.GetId();
            context.Add(restaurant);
            await context.SaveChangesAsync();

            // Criar categoria default do nosso restaurante
            context.Category.Add(new Category { Name = "Sem Categoria", IsDefault = true, RestaurantId = restaurant.Id });
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Reviewing));
        }

        /// <summary>
        /// Displays the reviewing page for a restaurant that is waiting for acceptance.
        /// Redirects to the index page if the restaurant is already accepted.
        /// </summary>
        /// <returns>
        /// The restaurant reviewing view or redirect to the index.
        /// </returns>
        public IActionResult Reviewing() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Create));
            if (MyRestaurant.Status != Enums.RestaurantStatus.WaitingAcceptance) return RedirectToAction(nameof(Index));
            return View();
        }

        /// <summary>
        /// Handles the editing of a restaurant's details.
        /// </summary>
        /// <param name="restaurant">The restaurant data to be updated.</param>
        /// <returns>
        /// A redirect to the index page after editing.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Name")] Restaurant restaurant) {
            if( MyRestaurant is null ) return RedirectToAction(nameof(Index));

            MyRestaurant.Name = restaurant.Name;
            context.Update(MyRestaurant);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays a list of all items in the restaurant, including those from all categories.
        /// </summary>
        /// <returns>
        /// A view displaying the restaurant's items.
        /// </returns>
        public IActionResult Items() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Index));

            List<Item> items = [];
            foreach (Category category in MyRestaurant.Categories ?? []) {
                items.AddRange(category.Items ?? []);
            }
            return View(items);
        }

        /// <summary>
        /// Displays the list of categories in the restaurant.
        /// </summary>
        /// <returns>
        /// A view displaying the restaurant's categories.
        /// </returns>
        public IActionResult Categories() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Index));
            return View(MyRestaurant.Categories);
        }

        /// <summary>
        /// Retorna a view com a listagem dos pedidos atuais que o meu restaurante recebeu.
        /// </summary>
        public IActionResult Orders() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Index));

            var current_orders = MyOrders?.Include(c => c.Items)
                .Where(c => c.Status >= Enums.OrderStatus.Placed && c.Status <= Enums.OrderStatus.Preparing)
                .OrderByDescending(c => c.Status).ToList() ?? [];

            return View(current_orders);
        }

        /// <summary>
        /// Retorna a view com a listagem dos pedidos historico que o meu restaurante recebeu
        /// </summary>
        public IActionResult OrdersHistory() {
            if( MyRestaurant is null ) return RedirectToAction(nameof(Index));

            var history = MyOrders?.Include(c => c.Items)
                .Where(o => o.Status >= Enums.OrderStatus.OnTheWay)
                .OrderByDescending(o => o.Id).ToList() ?? [];

            return View(history);
        }

        /// <summary>
        /// Coloca o pedido recebido com o estado de "Preparando"
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> StartPreparing(int orderId)
        {
            var order = (MyOrders?.Where(o => o.Id == orderId && o.Status == Enums.OrderStatus.Placed).ToList() ?? []).First();
            if (order == null) return RedirectToAction(nameof(Orders));

            order.Status = Enums.OrderStatus.Preparing;
            context.Cart.Update(order);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Orders));
        }

        /// <summary>
        /// Coloca o pedido recebido com o estado de "Enviado"
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> SendOrder(int orderId)
        {
            var order = (MyOrders?.Where(o => o.Id == orderId && o.Status == Enums.OrderStatus.Preparing).ToList() ?? []).First();
            if (order == null) return RedirectToAction(nameof(Orders));

            order.Status = Enums.OrderStatus.OnTheWay;
            context.Cart.Update(order);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Orders));
        }

        /// <summary>
        /// Elimina restaurante e dados associados
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Remove(int id) {
            // Id inválido
            if (MyRestaurant?.Id != id) return RedirectToAction(nameof(Index));

            context.Restaurant.Remove(MyRestaurant);
            await context.SaveChangesAsync();

            // Restaurante removido, redirect para a página de criar novo
            return RedirectToAction(nameof(Create));
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(double lat, double lng, string fullAddress)
        {
            var address = await context.Address.FirstOrDefaultAsync(address => address.AccountId == MyRestaurant.Id);

            if (address == null)
            {
                address = await addressService.AddAddress(lat, lng, fullAddress, MyRestaurant.Id);
                MyRestaurant.AddressId = address.Id;
            }
            else
            {
                address.Latitude = lat;
                address.Longitude = lng;
                address.FullAddress = fullAddress;
            }

            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
