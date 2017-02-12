using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Equation
{
	public class ExpressionCalculator
	{
		internal class FunctionOrOperation
		{
			public FunctionOrOperation()
			{
				Parameters = 2;
			}

			internal string Term
			{
				get;
				set;
			}

			internal int Precedence
			{
				get;
				set;
			}

			internal string StopCondition
			{
				get;
				set;
			}

			internal Func<List<double>, double> Calc
			{
				get;
				set;
			}

			internal int Parameters
			{
				get;
				set;
			}

			internal AssociationEnum Association
			{
				get;
				set;
			}

			public ContextEnum Context
			{
				get;
				set;
			}

			internal enum AssociationEnum
			{
				LEFT_TO_RIGHT,
				RIGHT_TO_LEFT
			}

			[Flags]
			internal enum ContextEnum
			{
				NONE = 1,
				STACK = 2,
				POP = 4
			}
			public override string ToString()
			{
				return string.Format("[FunctionOrOperation: Term={0}, Context={1}, Association={2}]"
				                     , Term, Context, Association);
			}
		}

		readonly bool DEBUG = false;

		internal bool Solve(string expression, out double result)
		{
			var parsed = true;
			result = 0.0;
			try
			{
				var values = ExpressionToPostFix(expression);

				if (DEBUG)
				{
					Console.WriteLine("original:");
					Console.WriteLine(expression);
					Console.WriteLine("postfixed:");
					foreach (var item in values)
						Console.Write("{0} ", item is FunctionOrOperation ? ((FunctionOrOperation)item).Term : item);
					Console.WriteLine();
					Console.WriteLine();
				}

				result = Calculate(values);

			}
			catch 
			{
				parsed = false;
			}
			return parsed;
		}

		public ExpressionCalculator(bool debug = false)
		{
			DEBUG = debug;
				
			#region Brackets
			// left parenthesis
			_functionsOrOperations.Add(new FunctionOrOperation()
			{
				Term = "(",
				Precedence = 1000,
				Association = FunctionOrOperation.AssociationEnum.LEFT_TO_RIGHT,
				Context = FunctionOrOperation.ContextEnum.STACK 
                        | FunctionOrOperation.ContextEnum.NONE,
			});

			// right parenthesis
			_functionsOrOperations.Add(new FunctionOrOperation()
			{
				Term = ")",
				Precedence = 1000,
				Association = FunctionOrOperation.AssociationEnum.LEFT_TO_RIGHT,
				Context = FunctionOrOperation.ContextEnum.POP,
				StopCondition = "("
			});
			#endregion

			// division
			_functionsOrOperations.Add(new FunctionOrOperation()
			{
				Term = "/",
				Precedence = 100,
				Association = FunctionOrOperation.AssociationEnum.LEFT_TO_RIGHT,
				Context = FunctionOrOperation.ContextEnum.STACK,
				Calc = (List<double> arg) => arg[1] / arg[0]
			});

			// multiplication
			_functionsOrOperations.Add(new FunctionOrOperation()
			{
				Term = "*",
				Precedence = 100,
				Association = FunctionOrOperation.AssociationEnum.LEFT_TO_RIGHT,
				Context = FunctionOrOperation.ContextEnum.STACK,
				Calc = (List<double> arg) => arg[0] * arg[1]
			});

			/// addition
			_functionsOrOperations.Add(new FunctionOrOperation()
			{
				Term = "+",
				Precedence = 50,
				Association = FunctionOrOperation.AssociationEnum.LEFT_TO_RIGHT,
				Context = FunctionOrOperation.ContextEnum.STACK,
				Calc = (List<double> arg) => arg[0] + arg[1]
			});

			/// subtraction
			_functionsOrOperations.Add(new FunctionOrOperation()
			{
				Term = "-",
				Precedence = 50,
				Association = FunctionOrOperation.AssociationEnum.LEFT_TO_RIGHT,
				Context = FunctionOrOperation.ContextEnum.STACK,
				Calc = (List<double> arg) => arg[0] - arg[1]
			});

			/// power
			_functionsOrOperations.Add(new FunctionOrOperation()
			{
				Term = "^",
				Precedence = 100,
				Association = FunctionOrOperation.AssociationEnum.RIGHT_TO_LEFT,
				Context = FunctionOrOperation.ContextEnum.STACK,
				Calc = (List<double> arg) => Math.Pow(arg[1], arg[0])
			});

			// square root
			_functionsOrOperations.Add(new FunctionOrOperation()
			{
				Term = "SQRT",
				Precedence = 100,
				Association = FunctionOrOperation.AssociationEnum.RIGHT_TO_LEFT,
				Context = FunctionOrOperation.ContextEnum.STACK,
				Calc = (List<double> arg) => Math.Sqrt(arg[0]),
				Parameters = 1
			});
		}

		List<FunctionOrOperation> _functionsOrOperations = new List<FunctionOrOperation>();

		Dictionary<string, FunctionOrOperation> _indexedFunOrOp;

		private Dictionary<string,FunctionOrOperation> Indexed
		{
			get
			{
				if (_indexedFunOrOp == null || _indexedFunOrOp.Count != _functionsOrOperations.Count)
					_indexedFunOrOp = _functionsOrOperations
						.ToDictionary(key => key.Term, value => value);
				return _indexedFunOrOp;
			}
		}

		List<object> ExpressionToPostFix(string expression)
		{
			var output = new List<object>();
			var stack = new Stack<FunctionOrOperation>();
			var token = new char[] { ' ' };
			var tokens = expression.Split(token, StringSplitOptions.RemoveEmptyEntries);
			foreach (var item in tokens)
			{
				FunctionOrOperation op;
				if (IsFunctionOrOperation(item, out op))
				{
					if (op.Context.HasFlag(FunctionOrOperation.ContextEnum.STACK))
					{
						while (stack.Count > 0 
						      && !stack.Peek().Context.HasFlag(FunctionOrOperation.ContextEnum.NONE)
							  && ((op.Association == FunctionOrOperation.AssociationEnum.LEFT_TO_RIGHT
								  && op.Precedence <= stack.Peek().Precedence)
								  || op.Association == FunctionOrOperation.AssociationEnum.RIGHT_TO_LEFT
								  && op.Precedence < stack.Peek().Precedence))
						{
							var poped = stack.Pop();
							if (!poped.Context.HasFlag(FunctionOrOperation.ContextEnum.NONE))
								output.Add(poped);
						}
						stack.Push(op);
					}
					else if (op.Context.HasFlag(FunctionOrOperation.ContextEnum.POP))
					{
						var found = false;
						while (stack.Count > 0 && !found)
						{
							var poped = stack.Pop();
							if (Equals(op.StopCondition, poped.Term))
								found = true;
							else
								output.Add(poped);
						}
					}
				}
				else if (IsVariableOrConstant(item))
					output.Add(item);				
			}
			while (stack.Count > 0)
			{
				output.Add(stack.Pop());
			}
			return output;
		}

		double Calculate(List<object> output)
		{
			var stack = new Stack<double>();
			foreach (var item in output)
			{
				if (item is FunctionOrOperation)
				{
					if(DEBUG)
					Console.WriteLine("Found? FunctionOrOperation.");

					var op = item as FunctionOrOperation;

					var parameters = new List<double>();
					for (int aux = 0; aux < op.Parameters; aux++)
					{
						var value = stack.Pop();
						parameters.Add(value);
						if (DEBUG) Console.WriteLine("Pop: {0}", value);
					}

					var result = op.Calc(parameters);

					if (DEBUG)
					{
						foreach (var parameter in parameters)
						{
							Console.Write("[{0}] ", parameter);
						}
						Console.Write("{0} ", op.Term);
						Console.WriteLine(" = {0}", result);
						Console.Write("Pushing result: {0} => [{0}]", result);
						foreach (var stacked in stack)
							Console.Write("{0} ", stacked);
						Console.WriteLine();
					}



					stack.Push(Convert.ToDouble(result));

					//if (op.Association == FunctionOrOperation.AssociationEnum.LEFT_TO_RIGHT)
					//{
					//}
					//else if (op.Association == FunctionOrOperation.AssociationEnum.RIGHT_TO_LEFT)
					//{
					//}
				}
				else
				{
					if (DEBUG)
					{
						Console.WriteLine("Found? VariableOrConstant.");
						Console.Write("Pushing: {0} => [{0}]", item);
						foreach (var stacked in stack)
							Console.Write("{0} ", stacked);
						Console.WriteLine();
					}
					stack.Push(Convert.ToDouble(item));
				}
			}
			return stack.Pop();
		}

		bool IsFunctionOrOperation(string item, out FunctionOrOperation op)
		{
			op = null;
			if (Indexed.TryGetValue(item, out op))
			{
				return true;
			}
			return false;
		}

		bool IsVariableOrConstant(string item)
		{
			return true;
		}
	}
}