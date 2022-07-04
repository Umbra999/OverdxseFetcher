using Newtonsoft.Json;
using System.Text;

namespace OverdxseFetcher
{
    internal class Main
    {
        private static string? Keyword;
        public static void Init()
        {
            Console.WriteLine("1 - Fetch All");
            Console.WriteLine("2 - Fetch Keyword");
            int choice = Convert.ToInt32(Console.ReadLine());

            if (choice == 2)
            {
                Console.WriteLine("Enter Keyword:");
                Keyword = Console.ReadLine();
            }

            Task.Run(() => LoopCollectionRequest(1));
            Task.Run(() => LoopCollectionRequest(2));
            Task.Run(() => LoopCollectionRequest(3));
            Thread.Sleep(-1);
        }

        private static async Task LoopCollectionRequest(int page)
        {
            for (; ; )
            {
                HttpClient CurrentClient = new(new HttpClientHandler { UseCookies = false });
                HttpRequestMessage Payload = new(HttpMethod.Get, $"https://overdxseclothing.com/collections/all?page={page}&sort_by=created-descending");
                Payload.Headers.Add("User-Agent", RandomString(15));

                HttpResponseMessage Response = await CurrentClient.SendAsync(Payload);
                if (Response.IsSuccessStatusCode)
                {
                    string response = await Response.Content.ReadAsStringAsync();
                    HandleResponse(response);
                }
                await Task.Delay(random.Next(10000, 15000));
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
                    string URL = AllLine[i - 34].Split('"')[1];
                    Console.WriteLine($"{Name} for Price {Price}: https://overdxseclothing.com{URL}");

                    if (Keyword != null && !Name.Contains(Keyword)) continue;

                    if (!BlacklistedProducts.Contains(Name))
                    {
                        var ProductInfos = new
                        {
                            name = Name,
                            value = $"Price: **{Price}**"
                        };

                        var WebsiteInfos = new
                        {
                            name = "Link",
                            value = $"**https://overdxseclothing.com{URL}**"
                        };

                        object[] Fields = new object[]
                        {
                            ProductInfos,
                            WebsiteInfos
                        };

                        var Embed = new
                        {
                            title = Name.Contains("1of1") ? "New 1of1 Product" : "New limited Product",
                            color = 11342935,
                            fields = Fields
                        };

                        object[] Embeds = new object[]
                        {
                            Embed
                        };

                        Task.Run(() => SendEmbedWebHook("https://discord.com/api/webhooks/991884799398649997/ns7C8kCQS920qrVRju4lkDcBfyViP12WRfmX27DUa7C4Hx0uof4LW9IaSqEalYRtZcPn", Embeds));
                    }
                }
            }
        }

        private static readonly List<string> BlacklistedProducts = new()
        {
            "OVERDXSE CANVAS 1",
        };

        public static async Task SendEmbedWebHook(string URL, object[] MSG)
        {
            var req = new
            {
                content = "Pings: <@155552545782235137>",
                embeds = MSG
            };

            HttpClient CurrentClient = new(new HttpClientHandler { UseCookies = false });
            HttpRequestMessage Payload = new(HttpMethod.Post, URL)
            {
                Content = new StringContent(JsonConvert.SerializeObject(req), Encoding.UTF8, "application/json")
            };
            Payload.Headers.Add("User-Agent", RandomString(15));
            await CurrentClient.SendAsync(Payload);
        }

        private static readonly Random random = new(Environment.TickCount);

        public static string RandomString(int length)
        {
            char[] array = "abcdefghlijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToArray();
            string text = string.Empty;
            for (int i = 0; i < length; i++)
            {
                text += array[random.Next(array.Length)].ToString();
            }
            return text;
        }
    }
}
