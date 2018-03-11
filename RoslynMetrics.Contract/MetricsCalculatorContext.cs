using Microsoft.CodeAnalysis;
using System;
using System.Threading;

namespace RoslynMetrics.Contracts
{
	internal sealed class MetricsCalculatorContext : ICalculatorContext
	{
		private readonly SyntaxNode root;

		private readonly SemanticModel semanticModel;

		public SemanticModel Model
		{
			get
			{
				return this.semanticModel;
			}
		}

		public SyntaxNode Root
		{
			get
			{
				return this.root;
			}
		}

		public MetricsCalculatorContext(SemanticModel semanticModel)
		{
			this.semanticModel = semanticModel;
			SyntaxTree syntaxTree = this.semanticModel.SyntaxTree;
			this.root = syntaxTree.GetRoot(new CancellationToken());
		}
	}
}