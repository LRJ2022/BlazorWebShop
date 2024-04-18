using Microsoft.AspNetCore.Components;
using MyBlazorShopHosted.Libraries.Services.ShoppingCart;
using MyBlazorShopHosted.Libraries.Shared.ShoppingCart.Models;
using System;
using System.Text;
using System.Text.Json;

namespace BlazorApp1.Components.PayPalAPI
{
    public class PayPalAPICom
    {
        private string PaypalClientId = "";
        private string PaypalSecret = "";
        private string PaypalUrl = "";
        private decimal total;
        private ShoppingCartModel shoppingCart;
        private HttpClient httpClient = new HttpClient();

        // Constructor to inject dependencies
        public PayPalAPICom(IConfiguration config, ShoppingCartModel shoppingCartModel, decimal total)
        {
            shoppingCart = shoppingCartModel;
            this.total = total;

            PaypalClientId = config["PaypalSettings:CLIENT_ID"]!;
            PaypalSecret = config["PaypalSettings:Secret"]!;
            PaypalUrl = config["PaypalSettings:Url"]!;
        }

        public async Task<(string?, bool)> PPANewOrder()
        {
            string? accessToken = null;
            if (shoppingCart.Items.Count == 0 || (accessToken = await GETAccessToken(httpClient, PaypalClientId, PaypalSecret)) == null)
                return (accessToken == null ? "Error in accessToken fetch" : "Error in shoppingcart", false);
            var createOrderResponse = await POSTCreateOrder(httpClient, accessToken, PaypalUrl, shoppingCart);
            if (createOrderResponse == null)
                return ("Error creating order", false);

            string orderId = JsonDocument.Parse(await createOrderResponse.Content.ReadAsStringAsync()).RootElement.GetProperty("id").GetString();
            Console.WriteLine("Order created successfully " + orderId);
            var resone = await PPAFetchOrder(orderId);
            var captureLink = await PPACapturePayment(orderId);
            return captureLink;
            //save orderId to db
        }

        public async Task<(string?, bool)> PPAFetchOrder(string orderId)
        {
            string? accessToken;
            if (orderId == null || (accessToken = await GETAccessToken(httpClient, PaypalClientId, PaypalSecret)) == null)
                return (orderId == null ? "Error in accessToken fetch" : "No order id in API fetch", false);
            HttpResponseMessage? orderDetails;
            if ((orderDetails = await GETOrderDetails(httpClient, orderId, accessToken)) == null)
                return ("Error in GETOrderDetails", false);
            return (null, true);
        }

        public async Task<(string?, bool)> PPACapturePayment(string orderId)
        {
            string? accessToken;
            if (orderId == null || (accessToken = await GETAccessToken(httpClient, PaypalClientId, PaypalSecret)) == null)
                return (orderId == null ? "Error in accessToken fetch" : "No order id in API fetch", false);
            string? link = await POSTCapturePaymentForOrder(httpClient, orderId, accessToken);
            if (link == null)
                return ("Error capturing payment", false);
            return (link, true);
        }

        public async Task PPAListInvoices()
        {
            string? accessToken;
            if ((accessToken = await GETAccessToken(httpClient, PaypalClientId, PaypalSecret)) == null)
                return;
            if (await GETListInvoices(httpClient, accessToken) == null)
                Console.Error.WriteLine("Error listing invoices");
        }

        private static async Task<HttpResponseMessage?> POSTCreateOrder(HttpClient httpClient, string accessToken, string PaypalUrl, ShoppingCartModel shoppingCart)
        {
            var requestData = PayPalTemplates.CreateOrderTemplate(shoppingCart);
            var response = await PayPalTemplates.PostRequest(httpClient, accessToken, PaypalUrl + "/v2/checkout/orders", requestData);
            if (!response.IsSuccessStatusCode)
                return null;
            return response;
        }

        private static async Task<string?> POSTCapturePaymentForOrder(HttpClient httpClient, string orderId, string accessToken)
        {
            var requestData = PayPalTemplates.CaptureOrderTemplate();
            var response = await PayPalTemplates.PostRequest(httpClient, accessToken, "https://api-m.sandbox.paypal.com/v2/checkout/orders/" + orderId + "/confirm-payment-source/", requestData);
            //var response = await PayPalTemplates.PostRequest(httpClient, accessToken, "https://api-m.sandbox.paypal.com/v2/checkout/orders/" + orderId + "/capture", requestData);
            if (!response.IsSuccessStatusCode)
                return null;
            var responseContent = await response.Content.ReadAsStringAsync();
            var details = JsonDocument.Parse(responseContent).RootElement;
            var links = details.GetProperty("links").EnumerateArray().Last().GetProperty("href").GetString();
            return links;
        }

        private static async Task<string?> GETAccessToken(HttpClient httpClient, string clientId, string clientSecret)
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en_US");

            var credentials = $"{clientId}:{clientSecret}";
            var base64Credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", base64Credentials);

            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };

            var response = await httpClient.PostAsync("https://api-m.sandbox.paypal.com/v1/oauth2/token", new FormUrlEncodedContent(requestData));

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonData = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
            var accessToken = jsonData.GetProperty("access_token").GetString();
            return accessToken;
        }

        private static async Task<HttpResponseMessage?> GETOrderDetails(HttpClient httpClient, string orderID, string accessToken)
        {
            var getOrderUrl = "https://api-m.sandbox.paypal.com/v2/checkout/orders/";
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync(getOrderUrl + orderID);
            if (!response.IsSuccessStatusCode)
                return null;
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var ordercontent = await response.Content.ReadAsStringAsync();
            var hello = PayPalTemplates.ConvertJsonToShoppingcart(ordercontent);
            return response;
        }

        private static async Task<HttpResponseMessage?> GETListInvoices(HttpClient httpClient, string accessToken)
        {
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en_US");
            var getInvoicesurl = "https://api-m.sandbox.paypal.com/v2/invoicing/invoices?total_required=true&fields=items";
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.GetAsync(getInvoicesurl);
            var responseContent = await response.Content.ReadAsStringAsync();
            return response;
        }

        private static async Task<HttpResponseMessage> GETTest(HttpClient httpClient, string orderID, string PaypalClientId, string PaypalSecret)
        {
            var getAuthorizeurl = "https://api-m.sandbox.paypal.com/v2/checkout/orders/" + orderID + "/authorize";
            var getInvoicesurl = "https://api-m.sandbox.paypal.com/v2/invoicing/invoices?total_required=true";
            var getOrderUrl = "https://api-m.sandbox.paypal.com/v2/checkout/orders/";
            var accessToken = await GETAccessToken(httpClient, PaypalClientId, PaypalSecret);

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var orderresponse = await httpClient.GetAsync(getOrderUrl + orderID);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            //var orderresponse = await httpClient.GetAsync("https://api-m.sandbox.paypal.com/v2/invoicing/invoices/?total_required=true");
            var xx = orderresponse;
            var ordercontent = await orderresponse.Content.ReadAsStringAsync();
            var response = await httpClient.GetAsync(getAuthorizeurl);
            var responseContent = await response.Content.ReadAsStringAsync();
            return response;
        }
    }
}
