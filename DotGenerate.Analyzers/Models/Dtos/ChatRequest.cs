using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DotGenerate.Analyzers.Models.Dtos
{
	public class ChatRequest
	{
		[JsonProperty("model")]
		public string Model { get; set; } = "gpt-3.5-turbo";
		
		[JsonProperty("messages")]
		public IList<ChatMessage> Messages { get; set; }
		
		[JsonProperty("functions")]
		public IList<FunctionMessage> Functions { get; set; }
		
		[JsonProperty("temperature")]
		public double? Temperature { get; set; }
		
		[JsonProperty("top_p")]
		public double? TopP { get; set; }
		
		[JsonProperty("n")]
		public int? NumChoicesPerMessage { get; set; }
		
		[JsonProperty("stop")]
		internal object CompiledStop
		{
			get
			{
				if (MultipleStopSequences?.Length == 1)
					return StopSequence;
				else if (MultipleStopSequences?.Length > 0)
					return MultipleStopSequences;
				else
					return null;
			}
		}
		
		[JsonIgnore]
		public string[] MultipleStopSequences { get; set; }
		
		[JsonIgnore]
		public string StopSequence
		{
			get => MultipleStopSequences?.FirstOrDefault() ?? null;
			set
			{
				if (value != null)
					MultipleStopSequences = new string[] { value };
			}
		}

		[JsonProperty("max_tokens")]
		public int? MaxTokens { get; set; }
		
		[JsonProperty("frequency_penalty")]
		public double? FrequencyPenalty { get; set; }

		[JsonProperty("presence_penalty")]
		public double? PresencePenalty { get; set; }
		
		[JsonProperty("logit_bias")]
		public IReadOnlyDictionary<string, float> LogitBias { get; set; }
		
		[JsonProperty("user")]
		public string user { get; set; }
	}
}