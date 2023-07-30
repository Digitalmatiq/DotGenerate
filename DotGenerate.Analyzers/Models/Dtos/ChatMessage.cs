using Newtonsoft.Json;

namespace DotGenerate.Analyzers.Models.Dtos
{
    public class ChatMessage
    {
        public ChatMessage()
        {
            this.RawRole = "user";
        }

        [JsonProperty("role")]
        public string RawRole { get; set; }

        [JsonProperty("function_call")]

        public FunctionCall Call { get; set; }
        
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}