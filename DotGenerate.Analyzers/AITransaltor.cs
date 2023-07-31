using DotGenerate.Analyzers.Models;
using DotGenerate.Analyzers.Models.Dtos;
using DotGenerate.Analyzers.Models.Prompts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DotGenerate.Analyzers
{
	public class AITransaltor
	{
		private const string UserAgent = "dotgenerator/dotnet_openai_api";
		private string _key;

		public AITransaltor(string key)
		{
			this._key = key;
		}

		public string PrefixPersona { get; set; } =
			 "You will be my C# code translator today.";

		public string PrefixContext { get; set; } =
			 $"We will communicate only in json structure where I will send you my methods structure metadata in json format and your implementation response has to respect that as well. All my messages will be independent from one another so please do not remember anything between messages." +
			 $"The structure will be the following: {RequestJson} and {ResponseJson}. You have to write only the responses and you can use of the functions I provided, but in the end I expect a valid json. Now the real request jsons are: ";

		public string Name { get; set; } = "Compiler";

		private string Url { get => "https://api.openai.com/v1/chat/completions"; }

		private static string RequestJson => @"
{
    ""ReqId"": 1,
    ""Signature"": {
        ""Name"": ""NameOfMethod"",
        ""Parameters"": [
           ""//list of objects like the one below""
            {
                ""Name"": ""nameOfParameter"",
                ""Type"": ""TypeWrittenAsString""
            }
        ],
        ""ReturnType"": ""TypeWrittenAsString""
    },
    ""Description"": ""The method's purpose expressed as a string""
}";

		private static string ResponseJson => @"
{
    ""ReqId"": 1, ""//same id that matches the request of this response""
    ""Body"": ""Raw implementation written as text (with signature). Account for indentation as well"",
	 ""Namespaces"": ""All the required namespaces for the types found in this method""
}";

		public async Task<ClassPromptResponse> GetResponseFor(ClassPromptRequest codeRequest)
		{
			var methodResponses = new List<CodePromptResponse>();
			var usingNamespaces = new HashSet<string>();

			foreach (var method in codeRequest.MethodRequests)
			{
				var json = method.Serialize();
				var content = string.Concat(this.PrefixPersona, this.PrefixContext, json);

				var chatRequest = new ChatRequest
				{
					MaxTokens = 1024,
					Temperature = 0.1,
					PresencePenalty = 0,
					FrequencyPenalty = 0,
					Messages = new List<ChatMessage>
					{
						new ChatMessage
						{
							Name = this.Name,
							Content = content,
						}
					},
					Functions = new List<FunctionMessage>
					{
						new FunctionMessage
						{
							Name = "SerializeResponseToValidJson",
							Description = "Serialize the Method Response to the appropriate representation",
							Parameters = new Parameters
							{
								Type = "object",
								Properties = new Dictionary<string, Property>
								{
									{ 
										"id",
										new Property
										{
											Type = "integer",
											Description = "Id matching the request Id originally coupled with the request"
										}
									},
									{
										"bodyText", new Property
										{
											Type = "string",
											Description = "Raw implementation written as text (with full signature), but also accounting for indentation"
										}
									},
									{
										"namespaces", new Property
										{
											Type = "array",
											Items = new Property.ArrayItem
											{
												Type = "string",
											},
											Description = "All the namespaces required by this method in a 1d string array"
										}
									}
								}
							}
						}
					}
				};

				var resultAsString = await HttpRequest(this.Url, HttpMethod.Post, chatRequest).ConfigureAwait(false);
				var chatResult = JsonConvert.DeserializeObject<ChatResult>(resultAsString);

				var functionResults = chatResult.FunctionCallResults;

				var nsJson = functionResults["namespaces"].ToString().TrimStart('{').TrimEnd('}');
				var namespaces = JsonConvert.DeserializeObject<string[]>(nsJson); 

				var body = functionResults["bodyText"].ToString();
				var responseId = int.Parse(functionResults["id"].ToString());

				var response = new CodePromptResponse
				{
					Id = responseId,
					Body = body.StartsWith("public") ? body : $"{method.CodeSignature.FullName} {{ {body} }}"
				};

				methodResponses.Add(response);

				foreach (var ns in namespaces)
					usingNamespaces.Add(SanitizeNamespace(ns));
			}

			var classResponse = new ClassPromptResponse
			{
				Id = codeRequest.ReqId,
				Body = $"{codeRequest.CodeSignature.ClassName}{FormattingConstants.NewLine}{{{FormattingConstants.NewLine}{string.Join("", methodResponses.Select(m => $"{FormattingConstants.NewLine}{m.Body}{FormattingConstants.NewLine}"))}{FormattingConstants.NewLine}}}", 
				MethodResponses = methodResponses,
				MainNamespace = $"namespace {codeRequest.CodeSignature.Namespace};{FormattingConstants.NewLine}",
				UsingNamespaces = usingNamespaces.ToList()
			};

			return classResponse;
		}

		private static string SanitizeNamespace(string ns)
		{
			ns = ns.StartsWith("using") ? ns : $"using {ns}";
			ns = ns.EndsWith(";") ? ns : $"{ns};";

			return ns;
		}

		private HttpClient GetClient()
		{
			var client = new HttpClient();

			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this._key);
			client.DefaultRequestHeaders.Add("api-key", this._key);
			client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

			return client;
		}

		private async Task<string> HttpRequest(string url, HttpMethod verb, object postData)
		{
			var client = GetClient();

			string resultAsString = null;
			var req = new HttpRequestMessage(verb, url);

			if (postData != null)
			{
				var jsonContent = JsonConvert.SerializeObject(postData, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
				var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

				req.Content = stringContent;
			}

			var response = await client.SendAsync(req, HttpCompletionOption.ResponseContentRead);
			string errorMessage = null;

			try
			{
				resultAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			catch (Exception e)
			{
				errorMessage = e.Message;
			}
			finally
			{
				client.Dispose();
			}

			if (!response.IsSuccessStatusCode)
				throw new HttpRequestException(errorMessage);

			return resultAsString;
		}
	}
}