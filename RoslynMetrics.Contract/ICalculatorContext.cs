using Microsoft.CodeAnalysis;

namespace RoslynMetrics.Contracts
{
	public interface ICalculatorContext
	{
		SemanticModel Model
		{
			get;
		}

		SyntaxNode Root
		{
			get;
		}
	}
}