using System.Diagnostics;
using System.Text.Json.Nodes;

namespace AeroBites.Services
{
    public class PayPalService(IConfiguration config)
    {
        /// <summary>
        /// Queries PayPal API to create a Api Token for the application
        /// </summary>
        private async Task<bool> CreateApiToken()
        {
            var fetch_uri = config["PayPalSettings:Url"] + "/v1/oauth2/token";
            var fetch_content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "grant_type", "client_credentials" },
                { "ignoreCache", "true" },
                { "return_unconsented_scopes", "true" },
            });

            // Fetch to get the Api Token
            var response = await new Http(config, fetch_uri).BasicPost(fetch_content);

            // Json parses the response
            var json_response = JsonNode.Parse(response);
            if (json_response is null) return false;

            // Verifies if the token is something
            var token = json_response?["access_token"]?.ToString();
            if (token is "" || token is null) return false;

            // Save the token
            config["PayPalSettings:APIToken"] = token;
            return true;
        }

        /// <summary>
        /// Calls PayPal endpoint to validate if current Token is valid
        /// </summary>
        private async Task<bool> IsApiTokenValid()
        {
            var fetch_uri = config["PayPalSettings:Url"] + "/v1/oauth2/token";

            // Fetch to verify if API still valid
            string response = "";
            try
            {
                response = await new Http(config, fetch_uri).Get();
            }
            catch (Exception) { return false; }

            // Json parses the response
            var json_response = JsonNode.Parse(response);
            if (json_response is null) return false;
            if (json_response["expires_in"] is not null) return true;
            return false;
        }

        /// <summary>
        /// It will first query if the Token is valid, if is not create a token and save it
        /// 
        /// If its not possible to create a Api Token the application will throw a Exception
        /// </summary>
        private async Task<bool> ValidateApiToken()
        {
            if (await this.IsApiTokenValid()) return true;
            if (await this.CreateApiToken()) return true;
            throw new Exception("Cannot create PayPal Api Token");
        }

        private async Task<JsonNode> SetupPaymentMethod(string type = "CONSUMER")
        {
            // First validate current Token
            await this.ValidateApiToken();
            // Only after procceed with logic

            var fetch_uri = config["PayPalSettings:Url"] + "/v3/vault/setup-tokens";
            var fetch_content = JsonContent.Create(new {
                payment_source = new {
                    paypal = new {
                        permit_multiple_payment_tokens = true,
                        usage_pattern = "IMMEDIATE",
                        usage_type = "MERCHANT",
                        customer_type = type,
                        experience_context = new {
                            payment_method_preference = "IMMEDIATE_PAYMENT_REQUIRED",
                            brand_name = "AeroBites",
                            locale = "en-US",
                            return_url = type == "CONSUMER" ? config["PayPalSettings:ConsumerAddSuccess"] : config["PayPalSettings:BusinessAddSuccess"],
                            cancel_url = type == "CONSUMER" ? config["PayPalSettings:ConsumerAddCancel"] : config["PayPalSettings:BusinessAddCancel"],
                        }
                    }
                }
            });

            var response = await new Http(config, fetch_uri).Post(fetch_content);
            // Json parses the response
            return JsonNode.Parse(response ?? "{}") ?? "";
        }



        /// <summary>
        /// Queries PayPal API to start the flow of setting up the Client Payment Method
        /// </summary>
        /// <returns>The JSON returned by the API</returns>
        public async Task<JsonNode> SetupClientPaymentMethod() { return await this.SetupPaymentMethod(); }

        /// <summary>
        /// Queries PayPal API to start the flow of saving up the Business Payment Method
        /// </summary>
        /// <returns>The JSON returned by the API</returns>
        public async Task<JsonNode> SetupBusinessPaymentMethod() { return await this.SetupPaymentMethod("BUSINESS"); }

        /// <summary>
        /// Queries PayPal API to finished the flow of saving a Payment Method
        /// </summary>
        public async Task<JsonNode> CreatePaymentMethod(string token_id)
        {
            // First validate current Token
            await this.ValidateApiToken();
            // Only after procceed with logic

            var fetch_uri = config["PayPalSettings:Url"] + "/v3/vault/payment-tokens";
            var fetch_content = JsonContent.Create(new {
                payment_source = new {
                    token = new {
                        id = token_id,
                        type = "SETUP_TOKEN"
                    }
                }
            });

            var response = await new Http(config, fetch_uri).Post(fetch_content);
            return JsonNode.Parse(response ?? "{}") ?? "";
        }

        /// <summary>
        /// Queries PayPal API to delete the PaymentMethod
        /// </summary>
        public async Task<bool> DeletePaymentMethod(string token_id)
        {
            // First validate current Token
            await this.ValidateApiToken();
            // Only after procceed with logic

            var fetch_uri = config["PayPalSettings:Url"] + "/v3/vault/payment-tokens/" + token_id;

            await new Http(config, fetch_uri).Delete();
            return true;
        }




        /// <summary>
        /// Class to execute Http Requests to the PayPal API
        /// </summary>
        private class Http(IConfiguration config, string uri)
        {
            private bool _disposed = false;
            private readonly HttpClient _client = new() { BaseAddress = new Uri(uri) };


            /// <summary>
            /// Takes care of the request
            /// </summary>
            /// <returns>Request response</returns>
            private async Task<HttpResponseMessage> Request(HttpMethod method, HttpContent? content)
            {
                // Cannot execute another request with the same HttpClient
                if (this._disposed) throw new Exception("Cannot use the same Http Client");

                // Creates the request with the given properties
                var request = new HttpRequestMessage(method, this._client.BaseAddress) { Content = content };
                // Executes the request
                return await _client.SendAsync(request);
            }

            /// <summary>
            /// Takes care of the response
            /// </summary>
            /// <returns>String with the JSON response</returns>
            private async Task<string> Response(HttpResponseMessage response)
            {
                // After usage dispose of the client
                this._client.Dispose();
                this._disposed = true;

                // Throws if status code is not success
                // If there is any wierd bad request, Debug the response.Content.ReadAsStringAsync to see the content of the response
                if(!response.IsSuccessStatusCode) {
                    var problem = response.Content.ReadAsStringAsync();
                    Debug.WriteLine(problem);
                    response.EnsureSuccessStatusCode();
                }

                // Returns data
                return await response.Content.ReadAsStringAsync();
            }

            /// <summary>
            /// Executes a Post request with basic authorization
            /// </summary>
            public async Task<string> BasicPost(HttpContent content)
            {
                // Create the basic Authorization string
                var basic_auth = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(config["PayPalSettings:ClientId"] + ":" + config["PayPalSettings:ClientSecret"]));
                // Add the basic Authorization Header
                this._client.DefaultRequestHeaders.Add("Authorization", "Basic " + basic_auth);

                // Fetch and return response
                var response = await this.Request(HttpMethod.Post, content);
                return await this.Response(response);
            }

            /// <summary>
            /// Executes a Get request with the given content
            /// </summary>
            public async Task<string> Get(HttpContent? content = null)
            {
                // Add Headers
                this._client.DefaultRequestHeaders.Add("Authorization", "Bearer " + config["PayPalSettings:APIToken"]);
                this._client.DefaultRequestHeaders.Add("ContentType", "application/json");

                // Fetch and return response
                var response = await this.Request(HttpMethod.Get, content);
                return await this.Response(response);
            }

            /// <summary>
            /// Executes a Post request with the given content
            /// </summary>
            public async Task<string> Post(HttpContent? content = null)
            {
                // Add Headers
                this._client.DefaultRequestHeaders.Add("Authorization", "Bearer " + config["PayPalSettings:APIToken"]);
                this._client.DefaultRequestHeaders.Add("ContentType", "application/json");

                // Fetch and return response
                var response = await this.Request(HttpMethod.Post, content);
                return await this.Response(response);
            }


            /// <summary>
            /// Executes a Delete request with the given content
            /// </summary>
            public async Task<string> Delete(HttpContent? content = null)
            {
                // Add Headers
                this._client.DefaultRequestHeaders.Add("Authorization", "Bearer " + config["PayPalSettings:APIToken"]);
                this._client.DefaultRequestHeaders.Add("ContentType", "application/json");

                // Fetch and return response
                var response = await this.Request(HttpMethod.Delete, content);
                return await this.Response(response);
            }
        }
    }
}
