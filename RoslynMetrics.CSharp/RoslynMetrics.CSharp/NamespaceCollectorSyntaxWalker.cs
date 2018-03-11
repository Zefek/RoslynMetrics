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

namespace RoslynMetrics.CSharp
{
	internal sealed class NamespaceCollectorSyntaxWalker : CSharpSyntaxWalker, INamespaceCollector
	{
		private readonly IList<NamespaceDeclarationSyntax> namespaces = new List<NamespaceDeclarationSyntax>();

		public NamespaceCollectorSyntaxWalker() : base(0)
		{
		}

		public IEnumerable<T> GetNamespaces<T>(SyntaxNode commonNode)
		where T : SyntaxNode
		{
			SyntaxNode syntaxNode = commonNode;
			SyntaxNode syntaxNode1 = syntaxNode;
			if (syntaxNode != null)
			{
				this.Visit(syntaxNode1);
			}
			return this.namespaces.Cast<T>().ToList<T>().AsReadOnly();
		}

		public IEnumerable<NamespaceDeclarationSyntax> GetNamespaces(ICalculatorContext context)
		{
			SyntaxNode root = context.Root;
			SyntaxNode syntaxNode = root;
			if (root != null)
			{
				this.Visit(syntaxNode);
			}
			return this.namespaces.ToList<NamespaceDeclarationSyntax>().AsReadOnly();
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
		{
			base.VisitNamespaceDeclaration(node);
			this.namespaces.Add(node);
		}
	}
}