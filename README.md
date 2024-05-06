## Super Simple LLM Chat App 

### Setup
- Install python and [trafilatura library](https://trafilatura.readthedocs.io/en/latest/installation.html), config PATH for trafilatura
- Config your OPENAI api key into appsettings.json and run
- If you want to run with LLAMA 3 (hosting on Groq), just put a [Groq API Key](https://console.groq.com/keys) and set `UseGroq=true` in appsettings.json

```json
{
  "UseGroq": false,
  "OpenAiApiKey": "sk-xxx",
  "GroqApiKey": "gsk_xxx"
}
```

### Demo

https://github.com/minhhungit/SimpleFeedlyChatDemo/assets/2279508/554fc928-ddbe-499e-a5a4-b49e43e2af07

### Abilities
- Lấy thông tin thời gian
- Tìm RSS từ URL của trang web
- Thu thập và tóm tắt nội dung từ RSS
- Thu thập và tóm tắt nội dung từ trang web
- Tóm tắt văn bản
- ...

