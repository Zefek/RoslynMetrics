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
	internal sealed class StatementsAnalyzer : CSharpSyntaxWalker, IStatementCounterMetricProvider
	{
		private int counter;

		public StatementsAnalyzer() : base(0)
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
			this.CalculateConstructorStatements(node);
			this.CalculateCompilerGeneratedPropertyStatements(node);
			return this.counter;
		}

		private void CalculateCompilerGeneratedPropertyStatements(MemberNode node)
		{
			switch (node.Kind)
			{
				case MemberKind.GetProperty:
				case MemberKind.SetProperty:
				{
					if (MemberBodySelector.FindBody(node) == null)
					{
						StatementsAnalyzer statementsAnalyzer = this;
						statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
					}
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private void CalculateConstructorStatements(MemberNode node)
		{
			if (node.Kind == MemberKind.Constructor)
			{
				ConstructorDeclarationSyntax syntaxNode = node.SyntaxNode as ConstructorDeclarationSyntax;
				ConstructorDeclarationSyntax constructorDeclarationSyntax = syntaxNode;
				if (syntaxNode != null && constructorDeclarationSyntax.Initializer != null)
				{
					this.Visit(constructorDeclarationSyntax.Initializer);
					StatementsAnalyzer statementsAnalyzer = this;
					statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
				}
			}
		}

		public override void VisitBreakStatement(BreakStatementSyntax node)
		{
			base.VisitBreakStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitContinueStatement(ContinueStatementSyntax node)
		{
			base.VisitContinueStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitDoStatement(DoStatementSyntax node)
		{
			base.VisitDoStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitEmptyStatement(EmptyStatementSyntax node)
		{
			base.VisitEmptyStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitExpressionStatement(ExpressionStatementSyntax node)
		{
			base.VisitExpressionStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitFixedStatement(FixedStatementSyntax node)
		{
			base.VisitFixedStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitForEachStatement(ForEachStatementSyntax node)
		{
			base.VisitForEachStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitForStatement(ForStatementSyntax node)
		{
			base.VisitForStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitGlobalStatement(GlobalStatementSyntax node)
		{
			base.VisitGlobalStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitGotoStatement(GotoStatementSyntax node)
		{
			base.VisitGotoStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitCheckedStatement(CheckedStatementSyntax node)
		{
			base.VisitCheckedStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitIfStatement(IfStatementSyntax node)
		{
			base.VisitIfStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitInitializerExpression(InitializerExpressionSyntax node)
		{
			base.VisitInitializerExpression(node);
			StatementsAnalyzer count = this;
			count.counter = checked(count.counter + node.Expressions.Count);
		}

		public override void VisitLabeledStatement(LabeledStatementSyntax node)
		{
			base.VisitLabeledStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			base.VisitLocalDeclarationStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitLockStatement(LockStatementSyntax node)
		{
			base.VisitLockStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitReturnStatement(ReturnStatementSyntax node)
		{
			base.VisitReturnStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitSwitchStatement(SwitchStatementSyntax node)
		{
			base.VisitSwitchStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitThrowStatement(ThrowStatementSyntax node)
		{
			base.VisitThrowStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
		{
			base.VisitUnsafeStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitUsingDirective(UsingDirectiveSyntax node)
		{
			base.VisitUsingDirective(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitUsingStatement(UsingStatementSyntax node)
		{
			base.VisitUsingStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitWhileStatement(WhileStatementSyntax node)
		{
			base.VisitWhileStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}

		public override void VisitYieldStatement(YieldStatementSyntax node)
		{
			base.VisitYieldStatement(node);
			StatementsAnalyzer statementsAnalyzer = this;
			statementsAnalyzer.counter = checked(statementsAnalyzer.counter + 1);
		}
	}
}