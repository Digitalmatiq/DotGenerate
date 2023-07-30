using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.Text;

namespace DotGenerate.Analyzers
{
	[Generator]
	public class AISourceGenerator : ISourceGenerator
	{
		private AITransaltor _transaltor;

		public void Execute(GeneratorExecutionContext context)
		{
			var options = context.AnalyzerConfigOptions.GlobalOptions;
			if (options.TryGetValue("build_property.OpenAIKey", out var key))
				this._transaltor = new AITransaltor(key);
			
			if (this._transaltor == null)
				return;
			
			var implBuilder = ImplementationBuilder.From(context.Compilation);
			var classRequestPrompts = implBuilder.GetClassPrompts();
			var responsePrompts = this._transaltor.GetResponseFor(classRequestPrompts).GetAwaiter().GetResult();

			foreach (var pair in responsePrompts)
			{
				var classRequest = pair.Key;
				var classResponse = pair.Value;
				var methods = classResponse.MethodResponses;

				var builder = new StringBuilder();

				builder.AppendLine(classRequest.CodeSignature.TypeNamespace);
				builder.AppendLine("{");
				builder.AppendLine(classRequest.CodeSignature.ClassName);
				builder.AppendLine("\t{");

				foreach (var method in methods)
					builder.AppendLine(method.Body);

				builder.AppendLine("\t}");
				builder.AppendLine("}");

				var source = builder.ToString();
				context.AddSource(classRequest.CodeSignature.ClassFile, SourceText.From(source, Encoding.UTF8));
			}
		}

		public void Initialize(GeneratorInitializationContext context)
		{
// #if DEBUG
// 			if (!Debugger.IsAttached)
// 			{
// 				Debugger.Launch(); // launches a debugger
// 			}
// #endif
		}
	}
}