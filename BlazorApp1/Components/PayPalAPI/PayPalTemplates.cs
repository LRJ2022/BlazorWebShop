using MyBlazorShopHosted.Libraries.Shared.Product.Models;
using MyBlazorShopHosted.Libraries.Shared.ShoppingCart.Models;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

namespace BlazorApp1.Components.PayPalAPI
{
    public static class PayPalTemplates
    {
        public static async Task<HttpResponseMessage> PostRequest(HttpClient httpClient, string accessToken, string url, object requestData)
        {
            var requestBody = System.Text.Json.JsonSerializer.Serialize(requestData);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, content);
            return response;
        }

        public static async Task<ShoppingCartModel?> ConvertJsonToShoppingcart(string response)
        {
            PayPalOrder order = Newtonsoft.Json.JsonConvert.DeserializeObject<PayPalOrder>(response);
            var cart = new ShoppingCartModel();
            decimal total = 0;
            foreach (var item in order.purchase_units)
            {
                cart.Items.Add(new ShoppingCartItemModel
                (
                    new ProductModel(item.reference_id, "Find product from db", 0, "find image from db"),
                    1
                ));
                total += Convert.ToDecimal(item.amount.value);
            }
            return cart;
        }

        public static object CreateOrderTemplate(ShoppingCartModel shoppingCart)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"intent\": \"CAPTURE\",\"purchase_units\": [");

            var items = new List<Item>
            {
                new Item { Name = "A Gumball for Your Thoughts Apron", Quantity = 2, UnitPrice = 24.00m, ReferenceId = "bubbles-gumball-apron" },
            };

            var requestData2 = string.Join(",", items.Select(item => $"{{\"item\": {{\"name\": \"{item.Name}\",\"description\": \"{item.Quantity}\",\"quantity\": \"{item.Quantity}\",\"unit_amount\": {{\"currency_code\": \"USD\",\"value\": \"{item.UnitPrice:F2}\"}}}},\"amount\": {{\"currency_code\": \"USD\",\"value\": \"{item.UnitPrice * item.Quantity:F2}\"}},\"reference_id\": \"{item.ReferenceId}\"}}"));
            sb.Append(requestData2);
            sb.Append("],\"payment_source\": {\"paypal\": {\"experience_context\": {\"payment_method_preference\": \"IMMEDIATE_PAYMENT_REQUIRED\",\"brand_name\": \"EXAMPLE INC\",\"locale\": \"en-US\",\"landing_page\": \"LOGIN\",\"user_action\": \"PAY_NOW\"},\"cart\": {\"level_2\": {\"invoice_id\": \"string\",\"tax_total\": {\"currency_code\": \"str\",\"value\": \"string\"}},\"level_3\": {\"ships_from_postal_code\": \"string\",\"line_items\": {\"name\": \"string\",\"quantity\": \"string\",\"description\": \"string\",\"sku\": \"string\",\"url\": \"http://example.com\",\"image_url\": \"http://example.com\",\"unit_amount\": {\"currency_code\": \"str\",\"value\": \"string\"},\"tax\": {\"currency_code\": \"str\",\"value\": \"string\"},\"upc\": {\"type\": \"UPC-A\",\"code\": \"string\"},\"commodity_code\": \"string\",\"unit_of_measure\": \"string\",\"discount_amount\": {\"currency_code\": \"str\",\"value\": \"string\"},\"total_amount\": {\"currency_code\": \"str\",\"value\": \"string\"}},\"shipping_amount\": {\"currency_code\": \"str\",\"value\": \"string\"},\"duty_amount\": {\"currency_code\": \"str\",\"value\": \"string\"},\"discount_amount\": {\"currency_code\": \"str\",\"value\": \"string\"},\"shipping_address\": {\"address_line_1\": \"string\",\"address_line_2\": \"string\",\"admin_area_2\": \"string\",\"admin_area_1\": \"string\",\"postal_code\": \"string\",\"country_code\": \"st\"}}}}}");

            return sb;


            //var requestData = new
            //{
            //    intent = "CAPTURE",
            //    purchase_units = shoppingCart.Items.Select(item => new
            //    {
            //        item = new
            //        {
            //            name = item.Product.Name,
            //            description = item.Quantity.ToString(),
            //            quantity = item.Quantity.ToString(),
            //            unit_amount = new
            //            {
            //                currency_code = "USD",
            //                value = item.Price.ToString("F2")
            //            }
            //        },
            //        amount = new
            //        {
            //            currency_code = "USD",
            //            value = (item.Price * item.Quantity).ToString("F2")
            //        },
            //        reference_id = item.Product.Slug,
            //    }).ToArray(),
            //    payment_source = new
            //    {
            //        paypal = new
            //        {
            //            experience_context = new
            //            {
            //                payment_method_preference = "IMMEDIATE_PAYMENT_REQUIRED",
            //                brand_name = "EXAMPLE INC",
            //                locale = "en-US",
            //                landing_page = "LOGIN",

            //                user_action = "PAY_NOW"
            //            },
            //            cart = new
            //            {
            //                level_2 = new
            //                {
            //                    invoice_id = "string",
            //                    tax_total = new
            //                    {
            //                        currency_code = "str",
            //                        value = "string"
            //                    }
            //                },
            //                level_3 = new
            //                {
            //                    ships_from_postal_code = "string",
            //                    line_items =
            //                            new
            //                            {
            //                                name = "string",
            //                                quantity = "string",
            //                                description = "string",
            //                                sku = "string",
            //                                url = new Uri("http://example.com"),
            //                                image_url = new Uri("http://example.com"),
            //                                unit_amount = new
            //                                {
            //                                    currency_code = "str",
            //                                    value = "string"
            //                                },
            //                                tax = new
            //                                {
            //                                    currency_code = "str",
            //                                    value = "string"
            //                                },
            //                                upc = new
            //                                {
            //                                    type = "UPC-A",
            //                                    code = "string"
            //                                },
            //                                commodity_code = "string",
            //                                unit_of_measure = "string",
            //                                discount_amount = new
            //                                {
            //                                    currency_code = "str",
            //                                    value = "string"
            //                                },
            //                                total_amount = new
            //                                {
            //                                    currency_code = "str",
            //                                    value = "string"
            //                                }
            //                            },
            //                    shipping_amount = new
            //                    {
            //                        currency_code = "str",
            //                        value = "string"
            //                    },
            //                    duty_amount = new
            //                    {
            //                        currency_code = "str",
            //                        value = "string"
            //                    },
            //                    discount_amount = new
            //                    {
            //                        currency_code = "str",
            //                        value = "string"
            //                    },
            //                    shipping_address = new
            //                    {
            //                        address_line_1 = "string",
            //                        address_line_2 = "string",
            //                        admin_area_2 = "string",
            //                        admin_area_1 = "string",
            //                        postal_code = "string",
            //                        country_code = "st"
            //                    }
            //                }
            //            }
            //        }
            //    }
            //};
            //var requestData4 = requestData1 + requestData2 + requestData3;
            ////convert to json
            //requestData4 = JsonConvert.SerializeObject(requestData4);
            //return requestData4;
        }

        public static object CaptureOrderTemplate()
        {
            var requestData = new
            {
                id = "5O190127TN364715T",
                status = "PAYER_ACTION_REQUIRED",
                payment_source = new
                {
                    paypal = new
                    {
                        name = new
                        {
                            given_name = "John",
                            surname = "Doe"
                        },
                        email_address = "customer@example.com"
                    }
                },
                payer = new
                {
                    name = new
                    {
                        given_name = "John",
                        surname = "Doe"
                    },
                    email_address = "customer@example.com"
                },
            };
            return requestData;
        }

        public class Item
        {
            public string Name { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public string ReferenceId { get; set; }
        }

        public class PayPalPaymentSource
        {
            public Dictionary<string, object> paypal { get; set; }
        }

        public class Amount
        {
            public string currency_code { get; set; }
            public string value { get; set; }
        }

        public class Payee
        {
            public string email_address { get; set; }
            public string merchant_id { get; set; }
            public DisplayData display_data { get; set; }
        }

        public class DisplayData
        {
            public string brand_name { get; set; }
        }

        public class PurchaseUnit
        {
            public string reference_id { get; set; }
            public Amount amount { get; set; }
            public Payee payee { get; set; }
        }

        public class Link
        {
            public string href { get; set; }
            public string rel { get; set; }
            public string method { get; set; }
        }

        public class PayPalOrder
        {
            public string id { get; set; }
            public string intent { get; set; }
            public string status { get; set; }
            public PayPalPaymentSource payment_source { get; set; }
            public List<PurchaseUnit> purchase_units { get; set; }
            public DateTime create_time { get; set; }
            public List<Link> links { get; set; }
        }
    }
}