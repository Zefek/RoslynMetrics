using Microsoft.CodeAnalysis;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RoslynMetrics.Contracts.Calculators
{
	public abstract class SemanticModelMetricsCalculator
	{
		private readonly ICalculatorContext context;

		protected ICalculatorContext Context
		{
			get
			{
				return this.context;
			}
		}

		protected SemanticModelMetricsCalculator(ICalculatorContext context)
		{
			this.context = context;
		}

		protected SemanticModelMetricsCalculator(SemanticModel semanticModel)
		{
			this.context = new MetricsCalculatorContext(semanticModel);
		}

		protected SymbolInfo GetSymbolInfo(SyntaxNode node)
		{
			return this.context.Model.GetSymbolInfo(node, new CancellationToken());
		}
	}
}