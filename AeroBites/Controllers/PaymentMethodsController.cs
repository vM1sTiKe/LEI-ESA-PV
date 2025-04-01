using System.Text.Json.Nodes;
using AeroBites.Data;
using AeroBites.Models;
using AeroBites.Services;
using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class PaymentMethodsController(AeroBitesContext context, IConfiguration config) : Controller
    {
        /// <summary>
        /// Holds a query to get all the User payment methods
        /// </summary>
        private IQueryable<PaymentMethod> MyMethods => context.PaymentMethod.Where(m => m.AccountId == User.GetId());

        public IActionResult Index()
        {
            var methods = MyMethods.OrderByDescending(m => m.IsDefault).ToList() ?? [];
            return View(methods);
        }

        /// <summary>
        /// Starts the flow to add a new payment method to the user
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Add()
        {
            // Chama o endpoint to PayPal para começar o flow de criar um método de pagamento
            JsonNode response = await new PayPalService(config).SetupClientPaymentMethod();
            // Redireciona para o URL dado pelo serviço do PayPal
            var url = config["PayPalSettings:AproveUrl"] + response["id"];
            // Redireciona para a página do PayPal
            return Redirect(url);
        }

        /// <summary>
        /// Completes the flow of the addition of the client payment method
        /// 
        /// This endpoint will be called by the redirect of the PayPal API
        /// </summary>
        /// <param name="approval_token_id">The token sent by the PayPal API</param>
        [HttpGet]
        public async Task<IActionResult> AproveAdd(string approval_token_id)
        {
            // Chama endpoint do PayPal para finalizar a criação do método de pagamento
            JsonNode response = await new PayPalService(config).CreatePaymentMethod(approval_token_id);
            if(response is null) return RedirectToAction(nameof(Index));

            // Recolhe informação da resposta
            var id = response["id"]?.ToString() ?? "";
            String email = response["payment_source"]?["paypal"]?["email_address"]?.ToString() ?? "";

            // Procura se já existe um método do user
            bool is_first_method = (MyMethods.Where(m => m.IsDefault == true).ToList().Count == 0) || false;

            // Cria método de pagamento
            context.PaymentMethod.Add(new PaymentMethod { Details = email, ApiToken = id, AccountId = User.GetId(), IsDefault = is_first_method });
            await context.SaveChangesAsync();

            // Redireciona para a página de listagem de métodos de pagamento do utilizador
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Cancels the flow of the addition of the client payment method
        /// </summary>
        [HttpGet]
        public IActionResult CancelAdd() { return RedirectToAction(nameof(Index)); }


        /// <summary>
        /// Removes from the PayPal API and from the DB the given payment method
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Remove(int id)
        {
            // Procura o método de pagamento
            var method = MyMethods.Where(m => m.Id == id).ToList().FirstOrDefault();
            if(method is null) return RedirectToAction(nameof(Index));

            // Apagar método de pagamento na API do PayPal
            await new PayPalService(config).DeletePaymentMethod(method.ApiToken);
            // Remover método de pagamento da DB
            context.PaymentMethod.Remove(method);
            await context.SaveChangesAsync();

            // Procura se existe um default (se existem métodos, tem que existir um)
            if (MyMethods.Where(m => m.IsDefault == true).ToList().Count == 0)
            {
                // Recolhe o primeiro metodo da lista e verifica se existe, se sim mete ele como default
                var first_method = MyMethods.FirstOrDefault();
                if(first_method is not null)
                {
                    first_method.IsDefault = true;
                    context.PaymentMethod.Update(first_method);
                    await context.SaveChangesAsync();
                }
            }

            // Redireciona para a página de listagem
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Sets the given payment method as the default
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Default(int id)
        {
            // Procura por todos os métodos do cliente que estejam com o default a true
            var default_methods = MyMethods.Where(m => m.IsDefault == true).ToList() ?? [];


            foreach (var method in default_methods) {
                method.IsDefault = false;
                context.PaymentMethod.Update(method);
                await context.SaveChangesAsync();
            }

            // Procura pelo método enviado para o endpoint
            var current_method = MyMethods.Where(m => m.Id == id).FirstOrDefault();
            // Se nao existir retorna diretamente para a listagem
            if(current_method == null) return RedirectToAction(nameof(Index));

            // Altera o método atual para o default
            current_method.IsDefault = true;
            context.PaymentMethod.Update(current_method);
            await context.SaveChangesAsync();

            // Retorna para a listagem
            return RedirectToAction(nameof(Index));
        }
    }
}
