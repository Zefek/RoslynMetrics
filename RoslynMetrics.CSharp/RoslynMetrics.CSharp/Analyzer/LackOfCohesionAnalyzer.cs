using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using RoslynMetrics.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics.CSharp.Analyzer
{
	internal class LackOfCohesionAnalyzer : CSharpSyntaxWalker, ILcomMetricsProvider
	{
		private readonly SemanticModel model;

		private readonly IDictionary<string, int> numFieldAccesses = new Dictionary<string, int>();

		private int numFields;

		private int numInstanceFields;

		private int numMethods;

		private INamedTypeSymbol typeSymbol;

		public LackOfCohesionAnalyzer(SemanticModel model) : base(0)
		{
			this.model = model;
		}

		public LcomMetrics Calculate(TypeDeclarationSyntax type)
		{
			SemanticModel semanticModel = this.model;
			CancellationToken cancellationToken = new CancellationToken();
			this.typeSymbol = semanticModel.GetDeclaredSymbol(type, cancellationToken);
			SimpleMemberCollectorSyntaxWalker simpleMemberCollectorSyntaxWalker = new SimpleMemberCollectorSyntaxWalker();
			IEnumerable<SyntaxNode> list = simpleMemberCollectorSyntaxWalker.GetMembers(this.model, type).ToList<SyntaxNode>();
			this.FilterInstanceFieldDeclarations(list.OfType<FieldDeclarationSyntax>().ToList<FieldDeclarationSyntax>());
			this.FilterInstanceAutoProperties(list.OfType<PropertyDeclarationSyntax>().ToList<PropertyDeclarationSyntax>());
			this.Visit(type);
			double num = (double)this.numFieldAccesses.Values.Sum();
			int num1 = checked(this.numMethods * this.numInstanceFields);
			double num2 = Math.Round(1 - num / (double)num1, 2);
			if (double.IsNaN(num2) || num2 < 0)
			{
				num2 = 0;
			}
			double num3 = ((double)this.numMethods - num / (double)this.numInstanceFields) / (double)(checked(this.numMethods - 1));
			num3 = Math.Round(num3, 2);
			if (double.IsNaN(num3) || num2 < 0)
			{
				num3 = 0;
			}
			LcomMetrics lcomMetric = new LcomMetrics()
			{
				Lcom = num2,
				LcomHendersonSellers = num3,
				NumFields = this.numFields,
				NumMethods = this.numMethods
			};
			return lcomMetric;
		}

		private void FilterAccessors(AccessorListSyntax accessorList, params SyntaxKind[] accessorKinds)
		{
            if (accessorList == null)
                return;
			SyntaxList<AccessorDeclarationSyntax> accessors = accessorList.Accessors;
			SyntaxKind[] syntaxKindArray = accessorKinds;
			for (int i = 0; i < (int)syntaxKindArray.Length; i++)
			{
				SyntaxKind syntaxKind = syntaxKindArray[i];
				AccessorDeclarationSyntax accessorDeclarationSyntax = ((IEnumerable<AccessorDeclarationSyntax>)accessors).SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax syntax) => syntax.Kind() == syntaxKind);
				if (accessorDeclarationSyntax != null && accessorDeclarationSyntax.Body != null)
				{
					LackOfCohesionAnalyzer lackOfCohesionAnalyzer = this;
					lackOfCohesionAnalyzer.numMethods = checked(lackOfCohesionAnalyzer.numMethods + 1);
					this.FilterFieldAccessExpressions(accessorDeclarationSyntax.Body);
				}
			}
		}

		private void FilterFieldAccessExpressions(SyntaxNode node)
		{
			if (node != null)
			{
				IEnumerable<MemberAccessExpressionSyntax> memberAccessExpressionSyntaxes = node.DescendantNodes(null, false).OfType<MemberAccessExpressionSyntax>();
				List<string> list = this.FilterFieldAccessExpressions(memberAccessExpressionSyntaxes).ToList<string>();
				foreach (string str in list.Distinct<string>().ToList<string>())
				{
					IDictionary<string, int> item = this.numFieldAccesses;
					IDictionary<string, int> strs = item;
					string str1 = str;
					item[str1] = checked(strs[str1] + 1);
				}
			}
		}

		private IEnumerable<string> FilterFieldAccessExpressions(IEnumerable<MemberAccessExpressionSyntax> memberAccessExpressions)
		{
			foreach (MemberAccessExpressionSyntax memberAccessExpression in memberAccessExpressions)
			{
				SemanticModel semanticModel = this.model;
				SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(memberAccessExpression, new CancellationToken());
				ISymbol symbol = symbolInfo.Symbol;
				if (symbol == null)
				{
					continue;
				}
				ISymbol symbol1 = symbol.ContainingSymbol;
				if (!this.typeSymbol.Equals(symbol1))
				{
					continue;
				}
				SymbolKind kind = symbol.Kind;
				if ((int)kind != 6 && (int)kind != 15)
				{
					continue;
				}
				string str = symbol.Name;
				if (!this.numFieldAccesses.ContainsKey(str))
				{
					continue;
				}
				yield return str;
			}
		}

		private void FilterInstanceAutoProperties(IEnumerable<PropertyDeclarationSyntax> nodes)
		{
			bool flag;
			foreach (PropertyDeclarationSyntax node in nodes)
			{
				if (((IEnumerable<SyntaxToken>)node.Modifiers).Any<SyntaxToken>((SyntaxToken x) => x.ValueText == "static"))
				{
					continue;
				}
                if (node.AccessorList == null)
                    continue;
				SyntaxList<AccessorDeclarationSyntax> accessors = node.AccessorList.Accessors;
				AccessorDeclarationSyntax accessorDeclarationSyntax = ((IEnumerable<AccessorDeclarationSyntax>)accessors).SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax syntax) => syntax.Kind() == SyntaxKind.GetAccessorDeclaration);
				AccessorDeclarationSyntax accessorDeclarationSyntax1 = ((IEnumerable<AccessorDeclarationSyntax>)accessors).SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax syntax) => syntax.Kind() == SyntaxKind.SetAccessorDeclaration);
				if (accessorDeclarationSyntax == null && accessorDeclarationSyntax1 == null)
				{
					continue;
				}
				if (accessorDeclarationSyntax == null || accessorDeclarationSyntax.Body == null)
				{
					flag = (accessorDeclarationSyntax1 == null ? true : accessorDeclarationSyntax1.Body == null);
				}
				else
				{
					flag = false;
				}
				if (!flag)
				{
					continue;
				}
				LackOfCohesionAnalyzer lackOfCohesionAnalyzer = this;
				lackOfCohesionAnalyzer.numFields = checked(lackOfCohesionAnalyzer.numFields + 1);
				ISymbol declaredSymbol = this.model.GetDeclaredSymbol(node, new CancellationToken());
				if (declaredSymbol == null)
				{
					continue;
				}
				this.numFieldAccesses[declaredSymbol.Name] = 0;
			}
		}

		private void FilterInstanceFieldDeclarations(IEnumerable<FieldDeclarationSyntax> nodes)
		{
			foreach (FieldDeclarationSyntax node in nodes)
			{
				LackOfCohesionAnalyzer lackOfCohesionAnalyzer = this;
				lackOfCohesionAnalyzer.numFields = checked(lackOfCohesionAnalyzer.numFields + 1);
				if (((IEnumerable<SyntaxToken>)node.Modifiers).Any<SyntaxToken>((SyntaxToken x) => x.ValueText == "static"))
				{
					continue;
				}
				LackOfCohesionAnalyzer lackOfCohesionAnalyzer1 = this;
				lackOfCohesionAnalyzer1.numInstanceFields = checked(lackOfCohesionAnalyzer1.numInstanceFields + 1);
				ISymbol declaredSymbol = node.GetDeclaredSymbol(this.model);
				if (declaredSymbol == null)
				{
					continue;
				}
				this.numFieldAccesses[declaredSymbol.Name] = 0;
			}
		}

		public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			LackOfCohesionAnalyzer lackOfCohesionAnalyzer = this;
			lackOfCohesionAnalyzer.numMethods = checked(lackOfCohesionAnalyzer.numMethods + 1);
			this.FilterFieldAccessExpressions(node);
			base.VisitConstructorDeclaration(node);
		}

		public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
		{
			LackOfCohesionAnalyzer lackOfCohesionAnalyzer = this;
			lackOfCohesionAnalyzer.numMethods = checked(lackOfCohesionAnalyzer.numMethods + 1);
			this.FilterFieldAccessExpressions(node);
			base.VisitDestructorDeclaration(node);
		}

		public override void VisitEventDeclaration(EventDeclarationSyntax node)
		{
			this.FilterAccessors(node.AccessorList, new SyntaxKind[] { SyntaxKind.AddAccessorDeclaration, SyntaxKind.RemoveAccessorDeclaration });
			base.VisitEventDeclaration(node);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			LackOfCohesionAnalyzer lackOfCohesionAnalyzer = this;
			lackOfCohesionAnalyzer.numMethods = checked(lackOfCohesionAnalyzer.numMethods + 1);
			this.FilterFieldAccessExpressions(node);
			base.VisitMethodDeclaration(node);
		}

		public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			this.FilterAccessors(node.AccessorList, new SyntaxKind[] { SyntaxKind.GetAccessorDeclaration, SyntaxKind.SetAccessorDeclaration });
			base.VisitPropertyDeclaration(node);
		}
	}
}