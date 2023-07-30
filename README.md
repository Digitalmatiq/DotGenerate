# DotGenerate
The purpose of this package is to forget about implementations and let the AI do them instead through code Source Generators. RIght now the project does make use of the OpenAI API, but it can be developed to be used with other LLMs 

### Getting Started

Add the following to your csproj file
```xml
<ItemGroup>
	<CompilerVisibleProperty Include="OpenAIKey"/>
</ItemGroup>

<PropertyGroup>
	<OpenAIKey>Your API Key</OpenAIKey>
</PropertyGroup>
```

### Actual usage

Create any interface where you annotate it with ```[AIGenerated]``` attribute and then write a description for the methods you want to be generated inside xml summary tags.

Example:

```csharp
/// <summary>
/// This interface will be automatically implemented by AI
/// </summary>
[AIGenerated]
public interface IExample
{
	/// <summary>
	/// This method should multiply 2 numbers
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public int Multiply(int x, int y);
}
```

Afterwards an implementation will be automatically generated for that interface with suffix "_AIGenerated" and can be used like so:

```csharp
var example = new IExample_AIGenerated();
Console.WriteLine(example.Multiply(2, 3));
```

### Donation

If you feel like helping me develop this package further [Buy me a cofee](https://bmc.link/radinoiudaz)