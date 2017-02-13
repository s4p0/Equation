using System;

namespace Equation
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			double result;
            string expression = string.Empty;
            //expression = "3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3";
            //expression = "10/-1*-2";
            //expression = "-2 + 5   - SQRT(9) + PI";
            //expression = "-PI";
            expression = "10-ABS(5-10)";
            //expression = "10-5";
            ExpressionCalculator calc = new ExpressionCalculator(true);
			if (calc.Solve(expression, out result))
			{
				Console.WriteLine("Final result: {0}", result);
			}
		}
	}
}
