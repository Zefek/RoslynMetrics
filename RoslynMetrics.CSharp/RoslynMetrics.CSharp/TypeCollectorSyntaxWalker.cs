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
	internal sealed class TypeCollectorSyntaxWalker : CSharpSyntaxWalker, ITypeCollector
	{
		private readonly IList<TypeDeclarationSyntax> types = new List<TypeDeclarationSyntax>();

		public TypeCollectorSyntaxWalker() : base(0)
		{
		}

		public IEnumerable<T> GetTypes<T>(SyntaxNode namespaceNode)
		where T : SyntaxNode
		{
			NamespaceDeclarationSyntax namespaceDeclarationSyntax = namespaceNode as NamespaceDeclarationSyntax;
			NamespaceDeclarationSyntax namespaceDeclarationSyntax1 = namespaceDeclarationSyntax;
			if (namespaceDeclarationSyntax != null)
			{
				this.Visit(namespaceDeclarationSyntax1);
			}
			return this.types.OfType<T>().ToList<T>().AsReadOnly();
		}

		public override void VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			base.VisitClassDeclaration(node);
			this.types.Add(node);
		}

		public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
		{
			base.VisitInterfaceDeclaration(node);
			this.types.Add(node);
		}

		public override void VisitStructDeclaration(StructDeclarationSyntax node)
		{
			base.VisitStructDeclaration(node);
			this.types.Add(node);
		}
	}
}