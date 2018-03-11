using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RoslynMetrics.CSharp
{
	internal sealed class MemberClassCouplingAnalyzer : ClassCouplingAnalyzerBase, IClassCouplingMetricProvider
	{
		private readonly IDictionary<SymbolKind, Action<ISymbol>> symbolHandlers;

		public MemberClassCouplingAnalyzer(Microsoft.CodeAnalysis.SemanticModel semanticModel) : base(semanticModel)
		{
			Dictionary<SymbolKind, Action<ISymbol>> symbolKinds = new Dictionary<SymbolKind, Action<ISymbol>>()
			{
				{ SymbolKind.NamedType, new Action<ISymbol>((ISymbol x) => base.FilterTypeSymbol((ITypeSymbol)x)) },
				{ SymbolKind.Parameter, new Action<ISymbol>((ISymbol x) => base.FilterTypeSymbol(((IParameterSymbol)x).Type)) },
				{ SymbolKind.Method, new Action<ISymbol>((ISymbol x) => base.FilterTypeSymbol(((IMethodSymbol)x).ContainingType)) },
				{ SymbolKind.Field, new Action<ISymbol>((ISymbol x) => base.FilterTypeSymbol(((IFieldSymbol)x).Type)) },
				{ SymbolKind.Property, new Action<ISymbol>((ISymbol x) => base.FilterTypeSymbol(((IPropertySymbol)x).ContainingType)) },
				{ SymbolKind.Event, new Action<ISymbol>((ISymbol x) => base.FilterTypeSymbol(((IEventSymbol)x).ContainingType)) },
				{ SymbolKind.Local, new Action<ISymbol>((ISymbol x) => base.FilterTypeSymbol(((ILocalSymbol)x).Type)) },
				{ SymbolKind.DynamicType, new Action<ISymbol>((ISymbol x) => base.FilterTypeSymbol(((IDynamicTypeSymbol)x).ContainingType)) }
			};
			this.symbolHandlers = symbolKinds;
		}

		public IEnumerable<string> Calculate(MemberNode memberNode)
		{
			Action<SyntaxNode> action;
			Dictionary<MemberKind, Action<SyntaxNode>> memberKinds = new Dictionary<MemberKind, Action<SyntaxNode>>()
			{
				{ MemberKind.Method, new Action<SyntaxNode>((SyntaxNode x) => this.CalculateMethodClassCoupling((MethodDeclarationSyntax)x)) },
				{ MemberKind.Constructor, new Action<SyntaxNode>((SyntaxNode x) => this.CalculateGenericMemberClassCoupling((MemberDeclarationSyntax)x)) },
				{ MemberKind.Destructor, new Action<SyntaxNode>((SyntaxNode x) => this.CalculateGenericMemberClassCoupling((MemberDeclarationSyntax)x)) },
				{ MemberKind.GetProperty, new Action<SyntaxNode>((SyntaxNode x) => this.CalculatePropertyClassCoupling((PropertyDeclarationSyntax)x, SyntaxKind.GetAccessorDeclaration)) },
				{ MemberKind.SetProperty, new Action<SyntaxNode>((SyntaxNode x) => this.CalculatePropertyClassCoupling((PropertyDeclarationSyntax)x, SyntaxKind.SetAccessorDeclaration)) },
				{ MemberKind.AddEventHandler, new Action<SyntaxNode>((SyntaxNode x) => this.CalculateEventClassCoupling((EventDeclarationSyntax)x, SyntaxKind.AddAccessorDeclaration)) },
				{ MemberKind.RemoveEventHandler, new Action<SyntaxNode>((SyntaxNode x) => this.CalculateEventClassCoupling((EventDeclarationSyntax)x, SyntaxKind.RemoveAccessorDeclaration)) }
			};
			Dictionary<MemberKind, Action<SyntaxNode>> memberKinds1 = memberKinds;
			SyntaxNode syntaxNode = memberNode.SyntaxNode;
			if (memberKinds1.TryGetValue(memberNode.Kind, out action))
			{
				action(syntaxNode);
			}
			return base.GetCollectedTypesNames();
		}

		private void CalculateEventClassCoupling(EventDeclarationSyntax syntax, SyntaxKind kind)
		{
			base.FilterType(syntax.Type);
			BlockSyntax accessor = MemberClassCouplingAnalyzer.GetAccessor(syntax.AccessorList, kind);
			BlockSyntax blockSyntax = accessor;
			if (accessor != null)
			{
				this.Visit(blockSyntax);
			}
		}

		private void CalculateGenericMemberClassCoupling(MemberDeclarationSyntax syntax)
		{
			this.Visit(syntax);
		}

		private void CalculateMethodClassCoupling(MethodDeclarationSyntax syntax)
		{
			this.Visit(syntax);
			base.FilterType(syntax.ReturnType);
		}

		private void CalculatePropertyClassCoupling(PropertyDeclarationSyntax syntax, SyntaxKind kind)
		{
			base.FilterType(syntax.Type);
			BlockSyntax accessor = MemberClassCouplingAnalyzer.GetAccessor(syntax.AccessorList, kind);
			BlockSyntax blockSyntax = accessor;
			if (accessor != null)
			{
				this.Visit(blockSyntax);
			}
		}

		private static BlockSyntax GetAccessor(AccessorListSyntax accessorList, SyntaxKind kind)
		{
			return accessorList.Accessors.SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax x) => x.Kind() == kind).Body;
		}

		public override void VisitIdentifierName(IdentifierNameSyntax node)
		{
			Action<ISymbol> action;
			base.VisitIdentifierName(node);
			SymbolInfo symbolInfo = base.SemanticModel.GetSymbolInfo(node, new CancellationToken());
			if (symbolInfo.Symbol != null)
			{
				ISymbol symbol = symbolInfo.Symbol;
				if (this.symbolHandlers.TryGetValue(symbol.Kind, out action))
				{
					action(symbol);
				}
			}
		}

		public override void VisitParameter(ParameterSyntax node)
		{
			base.VisitParameter(node);
			TypeSyntax type = node.Type;
			TypeSyntax typeSyntax = type;
			if (type != null)
			{
				base.FilterType(typeSyntax);
			}
		}
	}
}