using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.CSharp
{
	internal sealed class MethodParameterAnalyzer : CSharpSyntaxWalker
	{
		private int numParameters;

		public MethodParameterAnalyzer() : base(0)
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
			return this.numParameters;
		}

		public override void VisitParameter(ParameterSyntax node)
		{
			base.VisitParameter(node);
			MethodParameterAnalyzer methodParameterAnalyzer = this;
			methodParameterAnalyzer.numParameters = checked(methodParameterAnalyzer.numParameters + 1);
		}
	}
}