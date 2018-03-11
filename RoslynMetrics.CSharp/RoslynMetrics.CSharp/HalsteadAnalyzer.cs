using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics.CSharp
{
	internal sealed class HalsteadAnalyzer : CSharpSyntaxWalker, IHalsteadMetricsProvider
	{
		private IHalsteadMetrics metrics;

		public HalsteadAnalyzer() : base(0)
		{
		}

		public IHalsteadMetrics Calculate(MemberNode node)
		{
			BlockSyntax blockSyntax = MemberBodySelector.FindBody(node);
			BlockSyntax blockSyntax1 = blockSyntax;
			if (blockSyntax != null)
			{
				this.Visit(blockSyntax1);
				return this.metrics;
			}
			if (!this.CalculateGenericPropertyMetrics(node))
			{
				return new HalsteadMetrics();
			}
			return this.metrics;
		}

		private bool CalculateGenericPropertyMetrics(MemberNode node)
		{
			PropertyDeclarationSyntax syntaxNode = node.SyntaxNode as PropertyDeclarationSyntax;
			PropertyDeclarationSyntax propertyDeclarationSyntax = syntaxNode;
			if (syntaxNode != null)
			{
				bool flag = ((IEnumerable<SyntaxToken>)propertyDeclarationSyntax.Modifiers).Any<SyntaxToken>((SyntaxToken x) => x.ValueText == "static");
				if (MemberBodySelector.FindBody(node) == null)
				{
					switch (node.Kind)
					{
						case MemberKind.GetProperty:
						{
							this.metrics = (flag ? HalsteadMetrics.GenericStaticGetPropertyMetrics : HalsteadMetrics.GenericInstanceGetPropertyMetrics);
							return true;
						}
						case MemberKind.SetProperty:
						{
							this.metrics = (flag ? HalsteadMetrics.GenericStaticSetPropertyMetrics : HalsteadMetrics.GenericInstanceSetPropertyMetrics);
							return true;
						}
					}
				}
			}
			return false;
		}

		private static IDictionary<SyntaxKind, IList<string>> ParseTokens(IEnumerable<SyntaxToken> tokens, IEnumerable<SyntaxKind> filter)
		{
			IList<string> item;
			IDictionary<SyntaxKind, IList<string>> syntaxKinds = new Dictionary<SyntaxKind, IList<string>>();
			foreach (SyntaxToken token in tokens)
			{
				SyntaxKind syntaxKind = token.Kind();
				if (!filter.Any<SyntaxKind>((SyntaxKind x) => x == syntaxKind))
				{
					continue;
				}
				string valueText = token.ValueText;
				if (!syntaxKinds.TryGetValue(syntaxKind, out item))
				{
					syntaxKinds[syntaxKind] = new List<string>();
					item = syntaxKinds[syntaxKind];
				}
				item.Add(valueText);
			}
			return syntaxKinds;
		}

		public IHalsteadMetrics QueryMetrics()
		{
			return this.metrics;
		}

		public override void VisitBlock(BlockSyntax node)
		{
			base.VisitBlock(node);
			IEnumerable<SyntaxToken> list = node.DescendantTokens(null, false).ToList<SyntaxToken>();
			IDictionary<SyntaxKind, IList<string>> syntaxKinds = HalsteadAnalyzer.ParseTokens(list, HalsteadOperands.All);
			IDictionary<SyntaxKind, IList<string>> syntaxKinds1 = HalsteadAnalyzer.ParseTokens(list, HalsteadOperators.All);
			HalsteadMetrics halsteadMetric = new HalsteadMetrics()
			{
				NumOperands = syntaxKinds.Values.SelectMany<IList<string>, string>((IList<string> x) => x).Count<string>(),
				NumUniqueOperands = syntaxKinds.Values.SelectMany<IList<string>, string>((IList<string> x) => x).Distinct<string>().Count<string>(),
				NumOperators = syntaxKinds1.Values.SelectMany<IList<string>, string>((IList<string> x) => x).Count<string>(),
				NumUniqueOperators = syntaxKinds1.Values.SelectMany<IList<string>, string>((IList<string> x) => x).Distinct<string>().Count<string>()
			};
			this.metrics = halsteadMetric;
		}
	}
}