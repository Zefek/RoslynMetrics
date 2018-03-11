using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.CSharp
{
	internal sealed class MethodLocalVariablesAnalyzer : CSharpSyntaxWalker
	{
		private int numLocalVariables;

		public MethodLocalVariablesAnalyzer() : base(0)
		{
		}

		public int Calculate(SyntaxNode memberNode)
		{
			SyntaxNode syntaxNode = memberNode;
			SyntaxNode syntaxNode1 = syntaxNode;
			if (syntaxNode != null)
			{
				this.Visit(syntaxNode1);
			}
			return this.numLocalVariables;
		}

		public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
		{
			base.VisitVariableDeclaration(node);
			MethodLocalVariablesAnalyzer methodLocalVariablesAnalyzer = this;
			methodLocalVariablesAnalyzer.numLocalVariables = checked(methodLocalVariablesAnalyzer.numLocalVariables + 1);
		}
	}
}