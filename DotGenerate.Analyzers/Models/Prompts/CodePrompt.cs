using DotGenerate.Analyzers.Models.CodeParts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace DotGenerate.Analyzers.Models.Prompts
{
	public abstract class CodePromptRequest<TSignature>
		 where TSignature : CodeSignature
	{
		public int ReqId { get; set; }

		public TSignature CodeSignature { get; set; }

		public string Description { get; set; }

		public string Serialize() => $"\n\r{JsonConvert.SerializeObject(this)}\n\r";
	}

	public class CodePromptResponse
	{
		public int Id { get; set; }

		public List<string> Namespaces { get; set; }

		public string Body { get; set; }

		public string Source { get => $"{string.Join("", this.Namespaces.Select(ns => $"\r\n{ns}\r\n"))} {this.Body}"; }
	}
}