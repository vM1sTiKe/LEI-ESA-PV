using AeroBites.Services;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;
using AeroBites.Data;
using AeroBites.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace AeroBites.Controllers.MyRestaurant
{
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
            if(MyMethod.FirstOrDefault() is not null) return RedirectToAction("Index", "MyRestaurant");

            // TODO
            // Fazer como no PaymentMethods controller
            // Chamar await new PayPalService(config).SetupBusinessPaymentMethod(); em vez do método que está no outro lado
            // No fim alterar este redirect para ser como oq está no payment controller
            var url = "www.google.com";
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
            if (MyMethod.FirstOrDefault() is not null) return RedirectToAction("Index", "MyRestaurant");

            // TODO
            // mesma lógica do AproveAdd do payment controller
            // A unica diferença é ao adicionar o paymentmethod ao contexto não vamos linkar com um AccountId mas sim com RestaurantId
            // Nao é preciso inserir o IsDefault a true
            // Como tambem nao é preciso ir ao MyMethod validar se existe ou nao algum ja default


            // Redireciona para a página de editar o restaurante
            return RedirectToAction("Index", "MyRestaurant");
        }

        /// <summary>
        /// Cancels the flow of the addition of the restaurant paypal
        /// </summary>
        [HttpGet("CancelAdd")]
        public IActionResult CancelAdd() { return RedirectToAction("Index", "MyRestaurant"); }

        /// <summary>
        /// Removes from the PayPal API and from the DB
        /// </summary>
        [HttpPost("Remove")]
        public async Task<IActionResult> Remove(int id)
        {
            // Não pode remover se não exsitir restaurante
            if (MyRestaurant.FirstOrDefault() is null) return RedirectToAction("Index", "MyRestaurant");
            // Não pode remover se este nao existe paypal associado
            if (MyMethod.FirstOrDefault() is null) return RedirectToAction("Index", "MyRestaurant");

            // TODO
            // Validar que o MyMethod.Where o id enviado é o Id existente na BD
            // So se esse id for o correto que ele pode apagar
            // Se nao for o correto então return RedirectToAction("Index", "MyRestaurant");
            // Se então esse método existir chamar o serviço do paypal para remover esse paypal (igual ao payment controller)
            // Dps remover da BD (igual ao payment controller)


            // Redireciona para a página de editar o restaurante
            return RedirectToAction("Index", "MyRestaurant");
        }
    }
}
