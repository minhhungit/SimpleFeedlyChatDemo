using CodeHollow.FeedReader;
using Microsoft.SemanticKernel;
using System;
using System.ComponentModel;
using System.Text;

namespace SimpleFeedlyChatDemo.Functions
{
    [Description("Parse content from rss url and summarize")]
    public class RssParserAndSummarizePlugin
    {
        [KernelFunction, Description("Parse content from rss url and summarize")]
        public string CollectRssContent(
            Kernel kernel,
            [Description("rss url")] string rssUrl,
            [Description("number of rss item need to take")] int numberOfItems = 5)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"LOG: Đang thử lấy {numberOfItems} nội dung mới từ RSS url {rssUrl}");
            Console.ResetColor();

            var helper = new Helper();

            StringBuilder str = new StringBuilder();

            if (IsValidUrl(rssUrl))
            {
                Feed feed;
                try
                {
                    feed = FeedReader.ReadAsync(rssUrl).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    return $"Get error when trying to read rss from url {rssUrl}. ERROR: {ex.Message}";
                }

                var top = (feed?.Items ?? new List<FeedItem>()).Take(numberOfItems).ToList();

                var listProcesses = new List<Task>();
                var nbrOfWorkers = 3;

                var number = 0;
                foreach (FeedItem item in top)
                {
                    number++;

                    listProcesses.Add(Task.Run(async () =>
                    {
                        try
                        {
                            //var content = helper.ExtractUrlContentUsingTrafilatura(item.Link, includeImages: false) ?? string.Empty;

                            var content = item.Content;

                            if (string.IsNullOrWhiteSpace(content))
                            {
                                content = item.Description;
                            }

                            if (string.IsNullOrWhiteSpace(content))
                            {
                                content = item.Title;
                            }

                            var summarizedContent = string.Empty;

                            if (!string.IsNullOrWhiteSpace(content))
                            {
                                var summarizedResponse = await helper.SummarizeAsync(kernel, content);
                                summarizedContent = summarizedResponse.ToString();
                            }

                            if (!string.IsNullOrWhiteSpace(summarizedContent))
                            {
                                str.AppendLine($"[{item.Title}]({item.Link})");
                                str.AppendLine($"{summarizedContent}");
                                str.AppendLine("---");
                            }
                        }
                        catch { }
                    }));

                    if (number == nbrOfWorkers)
                    {
                        Task.WaitAll(listProcesses.ToArray());
                        number = 0;
                        listProcesses = new List<Task>();
                    }
                }

                if (listProcesses.Count() > 0)
                {
                    Task.WaitAll(listProcesses.ToArray());
                }

            }

            var result = str.ToString();

            return result;
        }

        private bool IsValidUrl(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult) &&
                          (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }
    }
}