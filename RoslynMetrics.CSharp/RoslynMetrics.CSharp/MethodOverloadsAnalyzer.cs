using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace RoslynMetrics.CSharp
{
	internal sealed class MethodOverloadsAnalyzer
	{
		private readonly SemanticModel model;

		public MethodOverloadsAnalyzer(SemanticModel model)
		{
			this.model = model;
		}

		public int Calculate(SyntaxNode node)
		{
			ISymbol declaredSymbol = ModelExtensions.GetDeclaredSymbol(this.model, node, new CancellationToken());
			if (declaredSymbol == null || (int)declaredSymbol.Kind != 9)
			{
				return 0;
			}
			ImmutableArray<ISymbol> members = declaredSymbol.ContainingType.GetMembers(declaredSymbol.Name);
			return members.Count<ISymbol>();
		}
	}
}