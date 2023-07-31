using Newtonsoft.Json;
using System.Collections.Generic;

namespace DotGenerate.Analyzers.Models.Dtos
{
	public class FunctionMessage
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("parameters")]
		public Parameters Parameters { get; set; }
	}

	public class Parameters
	{
		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("properties")]
		public Dictionary<string, Property> Properties { get; set; }
	}

	public class Property
	{
		public class ArrayItem
		{
			public string Type { get; set; }
		}

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("items")]
		public ArrayItem Items { get; set; }
	}
}