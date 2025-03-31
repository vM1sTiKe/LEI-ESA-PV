using AeroBites.Data;
using AeroBites.Models;
using Microsoft.AspNetCore.Mvc;

namespace AeroBites.Controllers
{
    public class PaymentMethodsController(AeroBitesContext context, IConfiguration config) : Controller
    {
        /// <summary>
        /// Holds a query to get all the User payment methods
        /// </summary>
        private IQueryable<PaymentMethod>? MyMethods => context.PaymentMethod.Where(m => m.AccountId == User.GetId());

        public IActionResult Index()
        {
            // TODO
            // Acrescentar um .orderby /.orderbydescending (nao sei qual é o correto) .orderby(m => m.IsDefault)
            // para devolver a listagem dos métodos do cliente onde em primeiro vem aquele que é default
            // Devolver a view enviando entao essa listagem
            return View();
        }


        [HttpPost]
        public IActionResult Add()
        {
            // TODO
            // Chamar o serviço do paypal SetupClientPaymentMethod()
            // Ao chamar este serviço agarrar na resposta do json json["id"]
            // concatenar json["id"] à string do appsettings config["PayPalSettings:AproveUrl"], isto vai criar o URL que temos que dar href
            // e com aquele href, em vez de fazer redirectToAction vamos é fazer redirect para o URL que queremos
            // O código acaba dps do redirect



            // Código para adicionar novo metodo
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Completes the flow of the addition of the client payment method
        /// 
        /// This endpoint will be called by the redirect of the PayPal API
        /// </summary>
        /// <param name="approval_token_id">The token sent by the PayPal API</param>
        [HttpGet]
        public IActionResult AproveAdd(string approval_token_id)
        {
            // TODO
            // Agarrar no approval token e chamar o serviço do paypal CreatePaymentMethod() enviando esse token para a funcao
            // Agarrar na resposta json["id"]
            // Agarrar no email do paypal utilizado json["payment_source"]["paypal"]["email_address"]
            // Criar um PaymentMethod (model) e associar ele ao utilizador atual (User.GetId())
            // O model do PaymentMethod precisa de details (email), do ApiToken (id) e associar entao o AccountId ao User.getid
            // After saving the PaymentMethod redirect to the listing page
            // O código acaba dps do redirect
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Cancels the flow of the addition of the client payment method
        /// </summary>
        [HttpGet]
        public IActionResult CancelAdd() { return RedirectToAction(nameof(Index)); }


        [HttpPost]
        public IActionResult Remove(int id)
        {
            // TODO
            // Acrescentar ao MyMethods um .Where para filtrar ainda mais para devolver o metodo com o id enviado
            // Com o objeto do método de pagamento entao chamamos o serviço do paypal DeletePaymentMethod() enviando o method.ApiToken para essa funcao
            // Dps de esperar a funcao acabar removemos o objeto do metodo da 
            // O código acaba dps do redirect
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Default(int id)
        {
            // TODO
            // acrescentar um .Where no Mymethod para procurar por metodos que sejam IsDefault == true
            // se esse objeto existir entao alterar o IsDefault para false e guardar o objeto
            // Dps entao no Mymethod adicionar fazer outra pesquisa com um .Where onde procura onde o id == id
            // Se esse objeto existir entao troca o IsDefault para true e salva
            // O código acaba dps do redirect

            // código para adicionar o método como default removendo o atual default.
            // se o metodo enviado ja for o default ele vai parar de ser default
            return RedirectToAction(nameof(Index));
        }
    }
}
