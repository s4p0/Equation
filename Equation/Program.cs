using System;

namespace Equation
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			//var expression = "3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3";
			//var expression = "10/-1*-2";
			//var expression = "-2 + 5   - SQRT(9) + PI";
			var expression = "-PI";
			ExpressionCalculator calc = new ExpressionCalculator(true);
			double result;
			if (calc.Solve(expression, out result))
			{
				Console.WriteLine("Final result: {0}", result);
			}
		}
	}
}
