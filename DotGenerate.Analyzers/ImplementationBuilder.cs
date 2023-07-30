using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DotGenerate.Analyzers.Models.CodeParts;
using DotGenerate.Analyzers.Models.Prompts;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DotGenerate.Analyzers
{
    public class ImplementationBuilder
    {
        private readonly Compilation _compilation;
        private Dictionary<(INamedTypeSymbol Symbol, string Summary), List<(IMethodSymbol Symbol, string Summary)>> _taggedInterfaceSymbols;

        private ImplementationBuilder(Compilation compilation)
        {
            this._compilation = compilation;
        }

        public static ImplementationBuilder From(Compilation compilation) => new ImplementationBuilder(compilation);

        public List<ClassPromptRequest> GetClassPrompts()
        {
            this._taggedInterfaceSymbols = this.GetTaggedInterfaceSymbols();
            var implementationRequests = new List<ClassPromptRequest>();
            var counter = 0;
            
            foreach (var pair in this._taggedInterfaceSymbols)
            {
                var (interfaceSymbol, interfaceSummary) = pair.Key;
                var methodSymbols = pair.Value;

                var className = interfaceSymbol.Name;
                var implementations = interfaceSymbol.Interfaces
                    .Select(i => i.Name)
                    .ToList();
                
                var methodRequests = new List<MethodPromptRequest>(methodSymbols.Count);
                
                foreach (var (method, methodSummary) in methodSymbols)
                {
                    var methodName = method.Name;
                    var parameters = method.Parameters
                        .Select(p => new Parameter { Name = p.Name, Type = p.Type.ToDisplayString() })
                        .ToList();
                    var returnType = method.ReturnType.ToDisplayString();

                    var methodSignature = new MethodSignature
                    {
                        Name = methodName,
                        Parameters = parameters,
                        ReturnType = returnType,
                        FullName = $"public {returnType} {methodName} ({string.Join(",", parameters.Select(p => $"{p.Type} {p.Name}"))})",
                    };

                    var methodRequest = new MethodPromptRequest
                    {
                        ReqId = ++counter,
                        CodeSignature = methodSignature,
                        Description = methodSummary
                    };
                    methodRequests.Add(methodRequest);
                }

                var classSignature = new ClassSignature
                {
                    Name = className,
                    FullName = interfaceSymbol.ToDisplayString(),
                    Namespace = interfaceSymbol.ContainingNamespace.ToDisplayString(),
                    Implementations = implementations
                };
                
                var classRequest = new ClassPromptRequest
                {
                    ReqId = ++counter,
                    Description = interfaceSummary,
                    CodeSignature = classSignature,
                    MethodRequests = methodRequests,
                };
                implementationRequests.Add(classRequest);
            }

            return implementationRequests;
        }

        private Dictionary<(INamedTypeSymbol Symbol, string Summary), List<(IMethodSymbol Symbol, string Summary)>> GetTaggedInterfaceSymbols()
        {
            var allNodes = this._compilation.SyntaxTrees.SelectMany(s => s.GetRoot().DescendantNodes());
            var interfacesToMethods = new Dictionary<(INamedTypeSymbol Symbol, string Summary), List<(IMethodSymbol, string)>>();

            var allInterfaces = allNodes.OfType<InterfaceDeclarationSyntax>();
            foreach (var interfaceNode in allInterfaces)
            {
                var model = this._compilation.GetSemanticModel(interfaceNode.SyntaxTree);
                var interfaceSymbol = model.GetDeclaredSymbol(interfaceNode);
				
                if (interfaceSymbol is null)
                    continue;

                var interfaceComments = interfaceSymbol.GetDocumentationCommentXml();
                var interfaceSummary = ExtractSummaryFromXmlComments(interfaceComments);
                
                var aiGeneratedAttributes = interfaceSymbol.GetAttributes()
                    .Where(attr => attr?.AttributeClass?.Name.Contains("AIGenerated") ?? false);

                if (aiGeneratedAttributes.Any())
                {
                    var methodsWithSummaries = new List<(IMethodSymbol, string)>();
                    var methods = interfaceSymbol.GetMembers().OfType<IMethodSymbol>().ToList();
                    
                    foreach (var methodSymbol in methods)
                    {
                        var methodComments = methodSymbol.GetDocumentationCommentXml();
                        var methodSummary = ExtractSummaryFromXmlComments(methodComments);
                        methodsWithSummaries.Add((methodSymbol, methodSummary));
                    }
                    
                    interfacesToMethods.Add((interfaceSymbol, interfaceSummary), methodsWithSummaries);
                }
            }

            return interfacesToMethods;
        }

        private static string ExtractSummaryFromXmlComments(string xmlComments)
        {
            try
            {
                var element = XElement.Parse(xmlComments);
                var summaryElement = element.Element("summary");

                if (summaryElement == null)
                    return string.Empty;

                return summaryElement.Value.Trim();
            }
            catch (System.Xml.XmlException)
            {
                return string.Empty;
            }
        }
    }
}