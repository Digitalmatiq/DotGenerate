using DotGenerate.Analyzers.Models.CodeParts;
using System.Collections.Generic;

namespace DotGenerate.Analyzers.Models.Prompts
{
	public class ClassPromptRequest : CodePromptRequest<ClassSignature>
	{
		public List<MethodPromptRequest> MethodRequests { get; set; }
	}

	public class ClassPromptResponse : CodePromptResponse
	{
		public List<CodePromptResponse> MethodResponses { get; set; }
	}

	public class ClassSignature : CodeSignature
	{
		public List<string> Implementations { get; set; }

		public string ClassName => $"public class {this.Name}_AIGenerated : {this.FullName}";

		public string ClassFile => $"{this.Name}_AIGenerated.cs";
	}
}