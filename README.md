# DotGenerate
The purpose of this package is to forget about implementations and let the AI do them instead through code Source Generators

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

Create any interface where you annotate it with ```csharp [AIGenerated]``` attribute and then write a description for the methods you want to be generated inside xml summary tags.

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

### Donation

If you feel like helping me develop this package further [Buy me a cofee](https://bmc.link/radinoiudaz)