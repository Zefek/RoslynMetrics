using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RoslynMetrics.CSharp
{
	internal sealed class TypeClassCouplingAnalyzer : ClassCouplingAnalyzerBase
	{
		public TypeClassCouplingAnalyzer(Microsoft.CodeAnalysis.SemanticModel semanticModel) : base(semanticModel)
		{
		}

		public IEnumerable<string> Calculate(TypeDeclarationSyntax typeNode)
		{
			this.Visit(typeNode);
			return base.GetCollectedTypesNames();
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			base.VisitClassDeclaration(node);
			if (node.BaseList != null)
			{
				INamedTypeSymbol namedTypeSymbol = (
					from x in (IEnumerable<BaseTypeSyntax>)node.BaseList.Types
					select ModelExtensions.GetSymbolInfo(base.SemanticModel, x, new CancellationToken())).Where<SymbolInfo>((SymbolInfo x) => {
					if (x.Symbol == null)
					{
						return false;
					}
					return (int)x.Symbol.Kind == 11;
				}).Select<SymbolInfo, ISymbol>((SymbolInfo x) => x.Symbol).OfType<INamedTypeSymbol>().FirstOrDefault<INamedTypeSymbol>();
				if (namedTypeSymbol != null)
				{
					base.FilterTypeSymbol(namedTypeSymbol);
				}
			}
		}

		public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			base.VisitFieldDeclaration(node);
			base.FilterType(node.Declaration.Type);
		}
	}
}