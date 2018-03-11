using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RoslynMetrics.CSharp
{
	internal sealed class DepthOfInheritanceAnalyzer
	{
		private readonly IEnumerable<TypeKind> inheritableTypes = new TypeKind[] { TypeKind.Class, TypeKind.Struct };

		private readonly SemanticModel semanticModel;

		public DepthOfInheritanceAnalyzer(SemanticModel semanticModel)
		{
			this.semanticModel = semanticModel;
		}

		public int Calculate(TypeDeclarationSyntax type)
		{
			int num = 0;
			if (type.BaseList != null)
			{
				SeparatedSyntaxList<BaseTypeSyntax>.Enumerator enumerator = type.BaseList.Types.GetEnumerator();
				while (enumerator.MoveNext())
				{
					BaseTypeSyntax current = enumerator.Current;
					SymbolInfo symbolInfo = ModelExtensions.GetSymbolInfo(this.semanticModel, current, new CancellationToken());
					for (INamedTypeSymbol i = symbolInfo.Symbol as INamedTypeSymbol; i != null; i = i.BaseType)
					{
						if (this.inheritableTypes.Any<TypeKind>((TypeKind x) => x == i.TypeKind))
						{
							num = checked(num + 1);
						}
					}
				}
			}
			return num;
		}
	}
}