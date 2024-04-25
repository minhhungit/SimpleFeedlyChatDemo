using System.Text.Json.Serialization;

namespace SimpleFeedlyChatDemo.Models
{
    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("delta")]
        public ChatMessage Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }

    public class ChatMessage
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class ChatCompletionRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("max_tokens")]
        public int MaxTokens { get; set; }

        [JsonPropertyName("messages")]
        public List<ChatMessage> Messages { get; set; }

        [JsonPropertyName("presence_penalty")]
        public double PresencePenalty { get; set; }

        [JsonPropertyName("stream")]
        public bool Stream { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("top_p")]
        public int TopP { get; set; }

        [JsonPropertyName("logprobs")]
        public bool Logprobs { get; set; }

        [JsonPropertyName("top_logprobs")]
        public int TopLogprobs { get; set; }
    }

    public class ChatCompletionResponse
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        //[JsonPropertyName("id")]
        //public string Id { get; set; }


        //[JsonPropertyName("created")]
        //public int Created { get; set; }

        //[JsonPropertyName("model")]
        //public string Model { get; set; }



        //[JsonPropertyName("usage")]
        //public Usage Usage { get; set; }

        //[JsonPropertyName("system_fingerprint")]
        //public object SystemFingerprint { get; set; }
    }

    public class EmbeddingRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("input")]
        public string Input { get; set; }
    }

    public class EmbeddingResponse
    {
        [JsonPropertyName("data")]
        public List<EmbeddingDataEntry> Data { get; set; }
    }

    public class EmbeddingDataEntry
    {
        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("embedding")]
        public double[] Embedding { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
