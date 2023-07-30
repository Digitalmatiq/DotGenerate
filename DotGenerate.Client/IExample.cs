
using DotGenerate.Attributes;

namespace DotGenerate.Client
{
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
}