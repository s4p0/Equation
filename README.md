# Equation
##Parse and calculate math expressions with constants and function.

A .NET (Mono compatible) C# implementation of [Shunting-yard Algorithm](https://en.wikipedia.org/wiki/Shunting-yard_algorithm)

This solution supports:

  * use of unary operations;
  * prioritizations using parenthesis;
  * easy to create new constant support (list below);
  * easy to create new function support (list below);
  * and, all done converting it to postfix format.
  
  
Functions supported:
  * basic math operations [+-*/];
  * sqrt;
  * power;
  * and, it is easy to implement others.
  
Constants supported:
  * E;
  * PI;
  * RAD;
  * and, it is easy to implement others.
  
  
Use:

```csharp

  ExpressionCalculator calc = new ExpressionCalculator();

  double result;
  var expression = "3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3";
  if (calc.Solve(expression, out result))
  {
    Console.WriteLine("Final result: {0}", result);
  } 
   ```

output: *Final result: 3.0001220703125*


  
  
ps.: it does not implement a full parenthesis check or any syntax analyze.
  

--- 
Proudly done using [Visual Studio for Mac (preview 3)](https://www.visualstudio.com/vs/visual-studio-mac/)
