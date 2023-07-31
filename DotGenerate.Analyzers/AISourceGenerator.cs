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
				this._transaltor = new AITransaltor(key, context);
			
			if (this._transaltor == null)
				return;
			
			var implBuilder = ImplementationBuilder.From(context.Compilation);
			var classRequestPrompts = implBuilder.GetClassPrompts();

			foreach (var prompt in classRequestPrompts)
			{
				var responsePrompt = this._transaltor.GetResponseFor(prompt).GetAwaiter().GetResult();
				context.AddSource(prompt.CodeSignature.ClassFile, SourceText.From(responsePrompt.Source, Encoding.UTF8));
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