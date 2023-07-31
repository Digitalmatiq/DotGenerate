
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

		/// <summary>
		/// Print X in console
		/// </summary>
		public void PrintX();

		/// <summary>
		/// Function should calculate the conjugate of a complex number where number
		/// </summary>
		/// <param name="realPart"></param>
		/// <param name="imaginaryPart"></param>
		/// <returns></returns>
		public (decimal, decimal) CalculateComplexNumberConjugate(decimal realPart, decimal imaginaryPart);
	}
}