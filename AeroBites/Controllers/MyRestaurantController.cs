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
        private Restaurant? MyRestaurant => context.Restaurant.Include(r => r.PaymentMethod).Include(r => r.Categories).ThenInclude(c => c.Items).FirstOrDefault(r => r.OwnerId == User.GetId());
        private IQueryable<Cart>? MyOrders => context.Cart.Where(c => MyRestaurant != null && c.RestaurantId == MyRestaurant.Id);


        /// <summary>
        /// Ação que apresenta a página principal do restaurante autenticado. 
        /// Redireciona para a view create se o restaurante ainda não estiver pronto.
        /// Define a morada ativa nas variáveis de ViewBag, ou usa coordenadas padrão se não existir.
        /// </summary>
        /// <returns>View com os dados do restaurante.</returns>
        public IActionResult Index() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Create));
            if (MyRestaurant.Status == Enums.RestaurantStatus.WaitingAcceptance) return RedirectToAction(nameof(Reviewing));

            var activeAddress = context.Address.FirstOrDefault(a => a.RestaurantId == MyRestaurant.Id && a.IsActive);
            if (activeAddress != null)
            {
                ViewBag.Latitude = activeAddress.Latitude;
                ViewBag.Longitude = activeAddress.Longitude;
                ViewBag.FullAddress = activeAddress.FullAddress;
            }
            else
            {
                ViewBag.FullAddress = "";
                ViewBag.Latitude = 38.52165;
                ViewBag.Longitude = -8.83977;
            }

                return View(MyRestaurant);
        }

        /// <summary>
        /// Mostra o formulário de criação do restaurante, caso ainda não exista.
        /// Caso o restaurante já exista, redireciona para a página principal (Index).
        /// </summary>
        /// <returns>View de criação ou redirecionamento para Index.</returns>
        public IActionResult Create() {
            return MyRestaurant is null ? View() : RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Cria um novo restaurante, associando-o ao utilizador autenticado e criando uma categoria default e uma morada.
        /// </summary>
        /// <param name="restaurant">Objeto com os dados do restaurante a ser criado.</param>
        /// <returns>Redireciona para a página de reviewing do restaurante após a criação.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name")] Restaurant restaurant) {
            restaurant.OwnerId = User.GetId();
            context.Add(restaurant);
            await context.SaveChangesAsync();

            // Criar categoria default do nosso restaurante
            context.Category.Add(new Category { Name = "Categoria Não Atribuída", IsDefault = true, RestaurantId = restaurant.Id });

            // Adicionar morada
            if (double.TryParse(Request.Form["Latitude"], out double lat) &&
                double.TryParse(Request.Form["Longitude"], out double lng) &&
                !string.IsNullOrWhiteSpace(Request.Form["FullAddress"]))
            {
                var fullAddress = Request.Form["FullAddress"];
                await addressService.AddAddressRestaurant(lat, lng, fullAddress, restaurant.Id);
            }

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Reviewing));
        }

        /// <summary>
        /// Mostra a página de revisão do restaurante, caso o restaurante exista e esteja no estado "WaitingAcceptance".
        /// Caso contrário, redireciona para a criação ou para a página principal, dependendo do estado do restaurante.
        /// </summary>
        /// <returns>View de revisão ou redirecionamento para criação ou página principal.</returns>
        public IActionResult Reviewing() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Create));
            if (MyRestaurant.Status != Enums.RestaurantStatus.WaitingAcceptance) return RedirectToAction(nameof(Index));
            return View();
        }


        /// <summary>
        /// Edita o nome e a morada do restaurante. Se a morada já existir, ela será atualizada, caso contrário, será criada uma nova.
        /// </summary>
        /// <param name="restaurant">Objeto com os dados atualizados do restaurante.</param>
        /// <returns>Redireciona para a página principal após as alterações.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit([Bind("Name")] Restaurant restaurant) {
            if( MyRestaurant is null ) return RedirectToAction(nameof(Index));

            MyRestaurant.Name = restaurant.Name;
            context.Update(MyRestaurant);

            if (double.TryParse(Request.Form["Latitude"], out double lat) &&
               double.TryParse(Request.Form["Longitude"], out double lng) &&
               !string.IsNullOrWhiteSpace(Request.Form["FullAddress"]))
            {
                var fullAddress = Request.Form["FullAddress"];
                var existingAddress = await context.Address.FirstOrDefaultAsync(a => a.RestaurantId == MyRestaurant.Id);

                if(existingAddress == null)
                {
                    await addressService.AddAddressRestaurant(lat, lng, fullAddress, MyRestaurant.Id);
                    TempData[Enums.MessageType.successMessage.ToString()] = "Nome e morada alterado.";
                }
                else
                {
                    existingAddress.Latitude = lat;
                    existingAddress.Longitude = lng;
                    existingAddress.FullAddress = fullAddress;
                    TempData[Enums.MessageType.successMessage.ToString()] = "Nome e morada alterado.";
                }
            }
            else
            {
                TempData[Enums.MessageType.successMessage.ToString()] = "Nome do restaurante alterado.";
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Mostra a lista de itens do restaurante, agrupados pelas categorias do restaurante.
        /// Se o restaurante não existir, redireciona para a página principal.
        /// </summary>
        /// <returns>View com a lista de itens do restaurante.</returns>
        public IActionResult Items() {
            if (MyRestaurant is null) return RedirectToAction(nameof(Index));

            List<Item> items = [];
            foreach (Category category in MyRestaurant.Categories ?? []) {
                items.AddRange(category.Items ?? []);
            }
            return View(items);
        }

        /// <summary>
        /// Mostra a lista com as categorias do restaurante.
        /// </summary>
        /// <returns>
        /// View com as categorias do restaurante.
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

            TempData[Enums.MessageType.successMessage.ToString()] = "O pedido irá ser preparado.";

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

            order.Status = Enums.OrderStatus.Waiting;
            context.Cart.Update(order);
            await context.SaveChangesAsync();

            TempData[Enums.MessageType.successMessage.ToString()] = "Pedido enviado.";

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

            TempData[Enums.MessageType.successMessage.ToString()] = "Restaurante eliminado.";

            // Restaurante removido, redirect para a página de criar novo
            return RedirectToAction(nameof(Create));
        }

        /// <summary>
        /// Adiciona uma morada ao restaurante.
        /// </summary>
        /// <param name="lat">Latitude da morada.</param>
        /// <param name="lng">Longitude da morada.</param>
        /// <param name="fullAddress">Morada.</param>
        /// <param name="restaurantId">ID do restaurante ao qual a morada será associada.</param>
        /// <returns>Redireciona para a página principal após adicionar a morada.</returns>
        [HttpPost]
        public async Task<IActionResult> AddRestaurantAddress(double lat, double lng, string fullAddress, int restaurantId)
        {
            await addressService.AddAddressRestaurant(lat, lng, fullAddress, restaurantId);
            TempData[Enums.MessageType.successMessage.ToString()] = "Morada do restaurante adicionada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Remove a morada associada ao restaurante.
        /// </summary>
        /// <returns>Redireciona para a página principal após remover a morada.</returns>
        [HttpPost]
        public async Task<IActionResult> RemoveAddress()
        {
            var address = await context.Address.FirstOrDefaultAsync(a => a.RestaurantId == MyRestaurant.Id && a.IsActive);
            if (address != null)
            {
                context.Address.Remove(address);
                await context.SaveChangesAsync();
                TempData[Enums.MessageType.successMessage.ToString()] = "Morada removida com sucesso.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
