namespace DotGenerate.Client
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var example = new IExample_AIGenerated();
			Console.WriteLine(example.Multiply(2, 3));

			example.PrintX();
		}
	}
}