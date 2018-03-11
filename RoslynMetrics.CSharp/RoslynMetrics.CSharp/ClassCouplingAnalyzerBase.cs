using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
	internal abstract class ClassCouplingAnalyzerBase : CSharpSyntaxWalker
	{
		private readonly Microsoft.CodeAnalysis.SemanticModel semanticModel;

		private readonly IDictionary<string, ITypeSymbol> types = new Dictionary<string, ITypeSymbol>();

		protected Microsoft.CodeAnalysis.SemanticModel SemanticModel
		{
			get
			{
				return this.semanticModel;
			}
		}

		protected ClassCouplingAnalyzerBase(Microsoft.CodeAnalysis.SemanticModel semanticModel) : base(0)
		{
			this.semanticModel = semanticModel;
		}

		protected void FilterType(TypeSyntax syntax)
		{
			if (syntax.Kind() == SyntaxKind.PredefinedType)
			{
				return;
			}
			SymbolInfo symbolInfo = this.SemanticModel.GetSymbolInfo(syntax, new CancellationToken());
			if (symbolInfo.Symbol != null && symbolInfo.Symbol.Kind == SymbolKind.NamedType)
			{
				this.FilterTypeSymbol((ITypeSymbol)symbolInfo.Symbol);
			}
		}

		protected void FilterTypeSymbol(ITypeSymbol symbol)
		{
			ITypeSymbol typeSymbol;
			if (symbol == null)
			{
				return;
			}
			TypeKind typeKind = symbol.TypeKind;
			switch (typeKind)
			{
                case TypeKind.Class:
                case TypeKind.Delegate:
                case TypeKind.Enum:
                case TypeKind.Interface:
				{
					string qualifiedName = TypeNameResolver.GetQualifiedName(symbol);
					if (this.types.TryGetValue(qualifiedName, out typeSymbol))
					{
						return;
					}
					this.types[qualifiedName] = symbol;
					return;
				}
                case TypeKind.Dynamic:
                case TypeKind.Error:
				{
					return;
				}
				default:
				{
					if (typeKind == TypeKind.TypeParameter)
					{
						return;
					}
					return;
				}
			}
		}

		protected IEnumerable<string> GetCollectedTypesNames()
		{
			return (
				from x in this.types
				select x.Key).ToList<string>().AsReadOnly();
		}
	}
}