using System.Collections.Generic;
using DotGenerate.Analyzers.Models.CodeParts;

namespace DotGenerate.Analyzers.Models.Prompts
{
    public class MethodPromptRequest : CodePromptRequest<MethodSignature>
    {
    }

    public class MethodSignature : CodeSignature
    {
        public List<Parameter> Parameters { get; set; }
        
        public string ReturnType { get; set; }
    }
}