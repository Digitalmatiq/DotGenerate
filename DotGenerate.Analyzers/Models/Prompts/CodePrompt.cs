using DotGenerate.Analyzers.Models.CodeParts;
using Newtonsoft.Json;

namespace DotGenerate.Analyzers.Models.Prompts
{
    public abstract class CodePromptRequest<TSignature>
        where TSignature : CodeSignature
    {
        public long ReqId { get; set; }
        
        public TSignature CodeSignature { get; set; }
        
        public string Description { get; set; }

        public string Serialize() => $"{JsonConvert.SerializeObject(this)}\n\r";
    }
    
    public class CodePromptResponse
    {
        public int Id { get; set; }
        
        public string Body { get; set; }
    }
}