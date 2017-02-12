using System;

namespace Equation
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var expression = "3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3";
			ExpressionCalculator calc = new ExpressionCalculator();
			double result;
			if (calc.Solve(expression, out result))
			{
				Console.WriteLine("Final result: {0}", result);
			}
		}
	}
}
