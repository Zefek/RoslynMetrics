using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.CSharp
{
	internal sealed class CyclomaticComplexityAnalyzer : CSharpSyntaxWalker, ICyclomaticComplexityMetricProvider
	{
		private int counter = 1;

		public CyclomaticComplexityAnalyzer() : base(0)
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
			int num = CyclomaticComplexityAnalyzer.CalculateLogicalComplexity(node);
			return checked(this.counter + num);
		}

		private static int CalculateLogicalComplexity(MemberNode node)
		{
			return (new LogicalComplexityAnalyzer()).Calculate(node);
		}

		public override void VisitBinaryExpression(BinaryExpressionSyntax node)
		{
			base.VisitBinaryExpression(node);
			if (node.OperatorToken.Kind() == SyntaxKind.QuestionQuestionToken)
			{
				CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
				cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
			}
		}

		public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
		{
			base.VisitConditionalExpression(node);
			if (node.QuestionToken.Kind() == SyntaxKind.QuestionToken && node.ColonToken.Kind() == SyntaxKind.ColonToken)
			{
				CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
				cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
			}
		}

		public override void VisitDoStatement(DoStatementSyntax node)
		{
			base.VisitDoStatement(node);
			CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
			cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
		}

		public override void VisitElseClause(ElseClauseSyntax node)
		{
			base.VisitElseClause(node);
			CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
			cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
		}

		public override void VisitForEachStatement(ForEachStatementSyntax node)
		{
			base.VisitForEachStatement(node);
			CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
			cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
		}

		public override void VisitForStatement(ForStatementSyntax node)
		{
			base.VisitForStatement(node);
			CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
			cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
		}

		public override void VisitIfStatement(IfStatementSyntax node)
		{
			base.VisitIfStatement(node);
			CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
			cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
		}

		public override void VisitInitializerExpression(InitializerExpressionSyntax node)
		{
			base.VisitInitializerExpression(node);
			CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
			cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
		}

		public override void VisitSwitchSection(SwitchSectionSyntax node)
		{
			base.VisitSwitchSection(node);
			CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
			cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
		}

		public override void VisitWhileStatement(WhileStatementSyntax node)
		{
			base.VisitWhileStatement(node);
			CyclomaticComplexityAnalyzer cyclomaticComplexityAnalyzer = this;
			cyclomaticComplexityAnalyzer.counter = checked(cyclomaticComplexityAnalyzer.counter + 1);
		}
	}
}