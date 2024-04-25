using Microsoft.SemanticKernel;

namespace SimpleFeedlyChatDemo
{
    public class Helper
    {
        public Helper()
        {
        }

        public async Task<FunctionResult> SummarizeAsync(Kernel kernel, string contentToSummarize)
        {
            return await kernel.InvokeAsync("SummarizePlugin", "Summarize", new() {
              { "input", contentToSummarize }
            });
        }
    }    
}
