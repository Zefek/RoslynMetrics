using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.CSharp
{
	internal sealed class SimpleMemberCollectorSyntaxWalker : CSharpSyntaxWalker
	{
		private readonly IList<SyntaxNode> members = new List<SyntaxNode>();

		public SimpleMemberCollectorSyntaxWalker() : base(0)
		{
		}

		public IEnumerable<SyntaxNode> GetMembers(SemanticModel semanticModel, TypeDeclarationSyntax type)
		{
			this.Visit(type);
			return this.members.ToList<SyntaxNode>().AsReadOnly();
		}

		public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			base.VisitConstructorDeclaration(node);
			this.members.Add(node);
		}

		public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
		{
			base.VisitDestructorDeclaration(node);
			this.members.Add(node);
		}

		public override void VisitEventDeclaration(EventDeclarationSyntax node)
		{
			base.VisitEventDeclaration(node);
			this.members.Add(node);
		}

		public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			base.VisitFieldDeclaration(node);
			this.members.Add(node);
		}

		public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			base.VisitMethodDeclaration(node);
			this.members.Add(node);
		}

		public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			base.VisitPropertyDeclaration(node);
			this.members.Add(node);
		}
	}
}