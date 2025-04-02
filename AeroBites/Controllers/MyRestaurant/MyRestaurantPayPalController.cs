using AeroBites.Services;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using AeroBites.Data;
using AeroBites.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Authorization;

namespace AeroBites.Controllers.MyRestaurant
{
    [Authorize]
    [Route("/MyRestaurant/PayPal")]
    public class MyRestaurantPayPalController(AeroBitesContext context, IConfiguration config) : Controller
    {
        private IQueryable<Restaurant> MyRestaurant => context.Restaurant.Where(r => r.OwnerId == User.GetId());
        /// <summary>
        /// Holds a query to get the restaurant payment method
        /// </summary>
        private IQueryable<PaymentMethod> MyMethod => context.PaymentMethod.Where(m => MyRestaurant.FirstOrDefault() != null && m.RestaurantId == MyRestaurant.First().Id);

        /// <summary>
        /// Starts the flow to add a paypal account to the restaurant
        /// </summary>
        [HttpPost("Add")]
        public async Task<IActionResult> Add()
        {
            // Não pode adicionar se não exsitir restaurante
            if(MyRestaurant.FirstOrDefault() is null) return RedirectToAction("Index", "MyRestaurant");
            // Não pode adicionar se este restaurante já tem um paypal associado
            if (MyMethod.FirstOrDefault() is not null)
            {
                TempData[Enums.MessageType.infoMessage.ToString()] = "Só é possível associar um método de pagamento.";
                return RedirectToAction("Index", "MyRestaurant");
            }
            
            // Chama o endpoint to PayPal para começar o flow de criar um método de pagamento
            JsonNode response = await new PayPalService(config).SetupBusinessPaymentMethod();
            // Redireciona para o URL dado pelo serviço do PayPal
            var url = config["PayPalSettings:AproveUrl"] + response["id"];
            
            // Redireciona para a página do PayPal
            return Redirect(url);
        }

        /// <summary>
        /// Completes the flow of the addition of the restaurant paypal
        /// 
        /// This endpoint will be called by the redirect of the PayPal API
        /// </summary>
        /// <param name="approval_token_id">The token sent by the PayPal API</param>
        [HttpGet("AproveAdd")]
        public async Task<IActionResult> AproveAdd(string approval_token_id)
        {
            // Não pode adicionar se não exsitir restaurante
            if (MyRestaurant.FirstOrDefault() is null) return RedirectToAction("Index", "MyRestaurant");
            // Não pode adicionar se este restaurante já tem um paypal associado
            if (MyMethod.FirstOrDefault() is not null)
            {
                TempData[Enums.MessageType.infoMessage.ToString()] = "Só é possível associar um método de pagamento.";
                return RedirectToAction("Index", "MyRestaurant");
            }

            // Chama endpoint do PayPal para finalizar a criação do método de pagamento
            JsonNode response = await new PayPalService(config).CreatePaymentMethod(approval_token_id);
            if (response is null)
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "Não foi possível adicionar o método de pagamento.";
                return RedirectToAction(nameof(Index));
            }
            // Recolhe informação da resposta
            var id = response["id"]?.ToString() ?? "";
            String email = response["payment_source"]?["paypal"]?["email_address"]?.ToString() ?? "";
            // Cria método de pagamento
            context.PaymentMethod.Add(new PaymentMethod { Details = email, ApiToken = id, RestaurantId = MyRestaurant.First().Id});
            await context.SaveChangesAsync();

            TempData[Enums.MessageType.successMessage.ToString()] = "Foi adicionado o método de pagamento.";

            // Redireciona para a página de editar o restaurante
            return RedirectToAction("Index", "MyRestaurant");
        }

        /// <summary>
        /// Cancels the flow of the addition of the restaurant paypal
        /// </summary>
        [HttpGet("CancelAdd")]
        public IActionResult CancelAdd() {
            TempData[Enums.MessageType.errorMessage.ToString()] = "A operação foi cancelada.";
            return RedirectToAction("Index", "MyRestaurant");
        }

        /// <summary>
        /// Removes from the PayPal API and from the DB
        /// </summary>
        [HttpPost("Remove")]
        public async Task<IActionResult> Remove(int id)
        {
            // Não pode remover se não exsitir restaurante
            if (MyRestaurant.FirstOrDefault() is null) return RedirectToAction("Index", "MyRestaurant");
            // Não pode remover se este nao existe paypal associado
            if (MyMethod.FirstOrDefault() is null)
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "Não existe nenhum método de pagamento associado.";
                return RedirectToAction("Index", "MyRestaurant");
            }

            // Procura o método de pagamento
            var method = MyMethod.Where(m => m.Id == id).ToList().FirstOrDefault();
            if (method is null)
            {
                TempData[Enums.MessageType.errorMessage.ToString()] = "Não existe nenhum método de pagamento associado.";
                return RedirectToAction("Index", "MyRestaurant");
            }
            // Apagar método de pagamento na API do PayPal
            await new PayPalService(config).DeletePaymentMethod(method.ApiToken);
            // Remover método de pagamento da DB
            context.PaymentMethod.Remove(method);
            await context.SaveChangesAsync();

            TempData[Enums.MessageType.successMessage.ToString()] = "O método de pagamento foi removido.";

            // Redireciona para a página de editar o restaurante
            return RedirectToAction("Index", "MyRestaurant");
        }
    }
}
