using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Net.Http;

namespace SimpleFeedlyChatDemo.Functions
{
    [Description("Find rss url by site url")]
    public class RssFinderPlugin
    {
        [KernelFunction, Description("Find rss url by site url")]
        public async Task<string> FindRssUrl(
            [Description("website url")] string url)
        {
            var result = string.Empty;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("LOG: Đang tìm kiếm RSS url...");
            Console.ResetColor();

            var key = StandardizeUrl(url);

            if (RssFakeDatabase.ContainsKey(key))
            {
                result = RssFakeDatabase[key];
            }
            else
            {
                var candidates = new List<string>
                    {
                        Combine(url, "rss"),
                        Combine(url, "rss.xml"),
                        Combine(url, "feed"),
                        Combine(url, "feed.xml"),
                        Combine(url, "atom.xml")
                    };

                url = await TryToDiscovery(candidates);

                if (!string.IsNullOrWhiteSpace(url))
                {
                    result = url;
                }
                else
                {
                    if (url.Contains("/feed") || url.Contains("/rss"))
                    {
                        result =  url;
                    }
                    else
                    {
                        result = $"Can not find rss link for website {url}";
                    }
                }
            }

            return result;
        }

        private async Task<string> TryToDiscovery(List<string> urls)
        {
            var tasks = new List<Task>();
            var candidateUrl = new List<string>();

            var timeout = TimeSpan.FromSeconds(5);

            foreach (var url in urls)
            {
                tasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        using (var client = HttpClientProvider.GetHttpClient())
                        {
                            client.Timeout = timeout;

                            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                            //var response = await _client.GetAsync(url);

                            if (response.IsSuccessStatusCode)
                            {
                                candidateUrl.Add(url);
                            }
                            else
                            {
                                await Task.Delay(timeout);
                            }
                        }
                    }
                    catch
                    {
                        await Task.Delay(timeout);
                    }
                }));
            }

            await Task.WhenAny(tasks);

            return candidateUrl.FirstOrDefault() ?? string.Empty;
        }

        private string Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return string.Format("{0}/{1}", uri1, uri2);
        }

        private Dictionary<string, string> RssFakeDatabase = new Dictionary<string, string>
        {
            {"damienbod.com", "https://damienbod.com/feed" },
            {"vnexpress.net", "https://vnexpress.net/rss/tin-moi-nhat.rss" },
            {"thegradient.pub", "https://thegradient.pub/rss" },
            {"tinhte.vn", "https://tinhte.vn/rss" }
        };

        private string StandardizeUrl(string url)
        {
            Uri uri = new Uri(url);

            string host = uri.Host;

            // Removing "www." if it exists
            if (host.StartsWith("www."))
            {
                host = host.Substring(4);
            }

            return host.ToLower().Trim();
        }
    }
}