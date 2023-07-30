namespace DotGenerate.Analyzers.Models.CodeParts
{
    public abstract class CodeSignature
    {
        public string Name { get; set; }
        
        public string FullName { get; set; }

        public string Namespace { get; set; }
        
        public string TypeNamespace => $"namespace {this.Namespace}";
    }
}