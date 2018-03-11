using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.CSharp
{
	internal sealed class LogicalComplexityAnalyzer : CSharpSyntaxWalker, ILogicalComplexityMetricProvider
	{
		private int counter;

		public LogicalComplexityAnalyzer() : base(0)
		{
		}

		public int Calculate(MemberNode node)
		{
			BlockSyntax blockSyntax = MemberBodySelector.FindBody(node);
			BlockSyntax blockSyntax1 = blockSyntax;
			if (blockSyntax != null)
			{
				this.Visit(blockSyntax1);
			}
			return this.counter;
		}

		public override void VisitBinaryExpression(BinaryExpressionSyntax node)
		{
			LogicalComplexityAnalyzer logicalComplexityAnalyzer;
			base.VisitBinaryExpression(node);
			SyntaxKind syntaxKind = node.Kind();
			switch (syntaxKind)
			{
				case SyntaxKind.LogicalOrExpression:
				case SyntaxKind.LogicalAndExpression:
				{
					logicalComplexityAnalyzer = this;
					logicalComplexityAnalyzer.counter = checked(logicalComplexityAnalyzer.counter + 1);
					return;
				}
				case SyntaxKind.BitwiseOrExpression:
				case SyntaxKind.BitwiseAndExpression:
				case SyntaxKind.ExclusiveOrExpression:
				case SyntaxKind.EqualsExpression:
				case SyntaxKind.NotEqualsExpression:
				case SyntaxKind.LessThanExpression:
				case SyntaxKind.LessThanOrEqualExpression:
				case SyntaxKind.GreaterThanExpression:
				case SyntaxKind.GreaterThanOrEqualExpression:
				{
					return;
				}
				default:
				{
					if (syntaxKind == SyntaxKind.LogicalNotExpression)
					{
						logicalComplexityAnalyzer = this;
						logicalComplexityAnalyzer.counter = checked(logicalComplexityAnalyzer.counter + 1);
						return;
					}
					return;
				}
			}
		}
	}
}