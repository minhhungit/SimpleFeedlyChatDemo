using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Diagnostics;

namespace SimpleFeedlyChatDemo.Functions
{
    [Description("lấy nội dung của một trang web")]
    public class GetUrlContentPlugin
    {
        public GetUrlContentPlugin()
        {

        }

        [KernelFunction, Description("lấy nội dung từ một trang web (nếu url không phải RSS link) using trafilatura python library")]
        public async Task<string> GetWebPageContent(
            Kernel kernel,
            [Description("url to get content, this IS NOT rss url")] string url,
            [Description("summarize content or not")] bool shouldSummarize
            )
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"LOG: Đang lấy thông tin từ website: " + url);
            Console.ResetColor();

            string args = $"-u \"{url}\"";
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "trafilatura", // Use appropriate shell for your system
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            Process process = new Process
            {
                StartInfo = processStartInfo
            };

            process.Start();

            string content = process.StandardOutput.ReadToEnd();

            process.WaitForExit();

            process.Close();

            if (shouldSummarize && !string.IsNullOrWhiteSpace(content))
            {
                var helper = new Helper();
                var summarizedResponse = await helper.SummarizeAsync(kernel, content);
                return summarizedResponse.ToString();
            }

            //if (!string.IsNullOrWhiteSpace(result))
            //{
            //    Console.WriteLine("Đã lấy được thông tin, đang xử lý...");
            //}

            return content;
        }
    }
}