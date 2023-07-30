using System.Collections.Generic;
using Newtonsoft.Json;

namespace DotGenerate.Analyzers.Models.Dtos
{
    public class ChatResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("choices")]
        public IReadOnlyList<ChatChoice> Choices { get; set; }

        public override string ToString() => Choices[0].ToString();

        public Dictionary<string, object> FunctionCallResults => Choices[0].Message.Call.Arguments;
    }
    
    public class ChatChoice
    {
        [JsonProperty("message")]
        public ChatMessage Message { get; set; }
        public override string ToString() => this.Message.Content;
    }

    public class FunctionCall
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("arguments")]
        public string ArgumentsJson { get; set; }
        
        public Dictionary<string, object> Arguments
        {
            get => JsonConvert.DeserializeObject<Dictionary<string, object>>(this.ArgumentsJson);
        }
    }
}