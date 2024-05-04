namespace SimpleFeedlyChatDemo
{
    public class ProxAIHandler(string proxyApiBase) : HttpClientHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri != null && request.RequestUri.Host.Equals("api.openai.com", StringComparison.OrdinalIgnoreCase))
            {
                request.RequestUri = new Uri($"{proxyApiBase}{request.RequestUri.PathAndQuery}");
                
                // Step 1: Get the current JSON body
                //var requestBody = request.Content.ReadAsStringAsync(cancellationToken).GetAwaiter().GetResult();

                //// Step 2: Deserialize the JSON body into a C# object
                //var jsonObject = JsonSerializer.Deserialize<dynamic>(requestBody);

                //// Step 3: Modify the object by adding a new JSON property
                //jsonObject["stream"] = "true";

                //// Step 4: Serialize the modified object back into JSON
                //string modifiedJsonBody = JsonSerializer.Serialize(jsonObject);

                //// Step 5: Set the modified JSON as the new content of the HttpRequestMessage
                //request.Content = new StringContent(modifiedJsonBody, System.Text.Encoding.UTF8, "application/json");

            }
            return base.SendAsync(request, cancellationToken);
        }
    }

}
