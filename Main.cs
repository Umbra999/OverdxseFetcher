using Newtonsoft.Json;
using System.Text;

namespace OverdxseFetcher
{
    internal class Main
    {
        public static void Init()
        {
            Task.Run(() => LoopHoodieRequest());
            Task.Run(() => LoopShirtRequest());
            Task.Run(() => LoopPantsRequest());
            Thread.Sleep(-1);
        }

        private static async Task LoopHoodieRequest()
        {
            for (; ; )
            {
                HttpClient CurrentClient = new(new HttpClientHandler { UseCookies = false });
                HttpRequestMessage Payload = new(HttpMethod.Get, $"https://overdxseclothing.com/collections/hoodies");
                Payload.Headers.Add("User-Agent", "LunaR");

                HttpResponseMessage Response = await CurrentClient.SendAsync(Payload);
                if (Response.IsSuccessStatusCode)
                {
                    string response = await Response.Content.ReadAsStringAsync();
                    HandleResponse(response);
                }
                await Task.Delay(6000);
            }
        }

        private static async Task LoopShirtRequest()
        {
            for (; ; )
            {
                HttpClient CurrentClient = new(new HttpClientHandler { UseCookies = false });
                HttpRequestMessage Payload = new(HttpMethod.Get, $"https://overdxseclothing.com/collections/shirts");
                Payload.Headers.Add("User-Agent", "LunaR");

                HttpResponseMessage Response = await CurrentClient.SendAsync(Payload);
                if (Response.IsSuccessStatusCode)
                {
                    string response = await Response.Content.ReadAsStringAsync();
                    HandleResponse(response);
                }
                await Task.Delay(6000);
            }
        }

        private static async Task LoopPantsRequest()
        {
            for (; ; )
            {
                HttpClient CurrentClient = new(new HttpClientHandler { UseCookies = false });
                HttpRequestMessage Payload = new(HttpMethod.Get, $"https://overdxseclothing.com/collections/pants");
                Payload.Headers.Add("User-Agent", "LunaR");

                HttpResponseMessage Response = await CurrentClient.SendAsync(Payload);
                if (Response.IsSuccessStatusCode)
                {
                    string response = await Response.Content.ReadAsStringAsync();
                    HandleResponse(response);
                }
                await Task.Delay(6000);
            }
        }

        private static void HandleResponse(string response)
        {
            string[] AllLine = response.Split('\n');

            for (int i = 0; i < AllLine.Length; i++)
            {
                if (AllLine[i].Contains("Regular price"))
                {
                    string Price = AllLine[i + 1].Trim(' ');
                    string Name = AllLine[i - 7].Trim(' ').Split('>', '<')[2];
                    Console.WriteLine($"{Name} for Price {Price}");

                    if (!BlacklistedProducts.Contains(Name))
                    {
                        var ProductInfos = new
                        {
                            name = Name,
                            value = $"Price: **{Price}**"
                        };

                        object[] Fields = new object[]
                        {
                            ProductInfos
                        };

                        var Embed = new
                        {
                            title = "Rare Product Found",
                            color = 11342935,
                            fields = Fields
                        };

                        object[] Embeds = new object[]
                        {
                            Embed
                        };

                        SendEmbedWebHook("ur webhook", Embeds);
                    }
                }
            }
        }

        private static readonly List<string> BlacklistedProducts = new()
        {
            //"OVERDXSE RHINESTONE LOGO HOODIE all sizes",
            //"Limited OVERDXSE PILLS LOGO TEE all sizes"
        };

        public static void SendEmbedWebHook(string URL, object[] MSG)
        {
            Task.Run(async delegate
            {
                var req = new
                {
                    embeds = MSG
                };

                HttpClient CurrentClient = new(new HttpClientHandler { UseCookies = false });
                HttpRequestMessage Payload = new(HttpMethod.Post, URL);
                string joinWorldBody = JsonConvert.SerializeObject(req);
                Payload.Content = new StringContent(joinWorldBody, Encoding.UTF8, "application/json");
                Payload.Headers.Add("User-Agent", "LunaR");
                HttpResponseMessage Response = await CurrentClient.SendAsync(Payload);
            });
        }
    }
}
