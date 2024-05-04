using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel;
using SimpleFeedlyChatDemo.Functions;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SimpleFeedlyChatDemo
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine(@"    ____  __                        _ __  __       __    __    __  ___
   / __ \/ /___ ___  __   _      __(_) /_/ /_     / /   / /   /  |/  /
  / /_/ / / __ `/ / / /  | | /| / / / __/ __ \   / /   / /   / /|_/ / 
 / ____/ / /_/ / /_/ /   | |/ |/ / / /_/ / / /  / /___/ /___/ /  / /  
/_/   /_/\__,_/\__, /    |__/|__/_/\__/_/ /_/  /_____/_____/_/  /_/   
              /____/                                                  ");

            // Build configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            string apikey = System.Environment.GetEnvironmentVariable("PRIVATE_OPENAI_KEY", EnvironmentVariableTarget.User)!;
            if (string.IsNullOrWhiteSpace(apikey))
            {
                apikey = configuration.GetSection("OpenAiApiKey").Value ?? string.Empty;
            }            

            if (string.IsNullOrWhiteSpace(apikey))
            {
                throw new Exception("Require openAI api key");
            }

            IKernelBuilder kb = Kernel.CreateBuilder();

            var modelName = "gpt-3.5-turbo";
            var useGroq = configuration.GetValue<bool>("UseGroq");
            var enableStreaming = true;

            if (useGroq)
            {
                apikey = System.Environment.GetEnvironmentVariable("PRIVATE_GROQ_KEY", EnvironmentVariableTarget.User)!;
                if (string.IsNullOrWhiteSpace(apikey))
                {
                    apikey = configuration.GetSection("GroqApiKey").Value ?? string.Empty;
                }

                modelName = "llama3-70b-8192";
                enableStreaming = false;
                kb.AddOpenAIChatCompletion(modelName, apikey, httpClient: new HttpClient(new ProxAIHandler("https://api.groq.com/openai")));
            }
            else
            {
                kb.AddOpenAIChatCompletion(modelName, apikey);
            }
            

            kb.Services.AddLogging(c => c.AddConsole().SetMinimumLevel(LogLevel.Error));

            kb.Plugins.AddFromType<TimePlugin>();
            kb.Plugins.AddFromType<RssFinderPlugin>();
            kb.Plugins.AddFromType<RssParserAndSummarizePlugin>();
            kb.Plugins.AddFromType<GetUrlContentPlugin>();

            kb.Plugins.AddFromPromptDirectory("./Plugins/SummarizePlugin");

            Kernel kernel = kb.Build();
            
            Console.WriteLine("Initializing - please wait...");
            Console.WriteLine();

            var ai = kernel.GetRequiredService<IChatCompletionService>();
            OpenAIPromptExecutionSettings settings = new() { ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions };

            ChatHistory chat = new(@$"Bạn là trợ lý AI - Default answer in Vietnamese");
            chat.AddUserMessage("Xin chào, hãy cho biết bạn có những khả năng gì, ứng với mỗi khả năng hãy đề xuất 1 hoặc 2 câu hỏi");

            if (!enableStreaming)
            {
                // due to it's tool and function call has not supported streaming
                Console.Write(await ai.GetChatMessageContentAsync(chat, settings, kernel));
            }
            else
            {
                await foreach (var message in ai.GetStreamingChatMessageContentsAsync(chat, settings, kernel))
                {
                    Console.Write(message);
                }
            }

            Console.WriteLine("\n\n");

            StringBuilder builder = new();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n> Question: ");
                Console.ResetColor();

                string question = Console.ReadLine()!;

                chat.AddUserMessage(question);

                builder.Clear();

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("LOG: AI đang suy nghĩ...");
                Console.ResetColor();

                if (!enableStreaming)
                {
                    var msg = await ai.GetChatMessageContentAsync(chat, settings, kernel);

                    Console.WriteLine(msg);
                    builder.AppendLine(msg.ToString());
                }
                else
                {
                    await foreach (var message in ai.GetStreamingChatMessageContentsAsync(chat, settings, kernel))
                    {
                        Console.Write(message);
                        builder.Append(message);
                    }
                }
                
                Console.WriteLine();

                var assistantMsg = builder.ToString();
                chat.AddAssistantMessage(assistantMsg);

                Console.WriteLine();
            }
        }
    }
}
