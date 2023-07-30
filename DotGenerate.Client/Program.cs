

namespace DotGenerate.Client
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			var ex = new IExample_AIGenerated();
			Console.WriteLine(ex.Multiply(2, 3));
		}
	}
}