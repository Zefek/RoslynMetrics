using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.CSharp
{
	internal sealed class MemberBodySelector
	{
		public MemberBodySelector()
		{
		}

		public static BlockSyntax FindBody(MemberNode node)
		{
			Func<SyntaxNode, BlockSyntax> func;
			Dictionary<MemberKind, Func<SyntaxNode, BlockSyntax>> memberKinds = new Dictionary<MemberKind, Func<SyntaxNode, BlockSyntax>>()
			{
				{ MemberKind.Method, new Func<SyntaxNode, BlockSyntax>((SyntaxNode x) => ((MethodDeclarationSyntax)x).Body) },
				{ MemberKind.Constructor, new Func<SyntaxNode, BlockSyntax>((SyntaxNode x) => ((ConstructorDeclarationSyntax)x).Body) },
				{ MemberKind.Destructor, new Func<SyntaxNode, BlockSyntax>((SyntaxNode x) => ((DestructorDeclarationSyntax)x).Body) },
				{ MemberKind.GetProperty, new Func<SyntaxNode, BlockSyntax>((SyntaxNode x) => MemberBodySelector.GetPropertyAccessorBody((PropertyDeclarationSyntax)x, SyntaxKind.GetAccessorDeclaration)) },
				{ MemberKind.SetProperty, new Func<SyntaxNode, BlockSyntax>((SyntaxNode x) => MemberBodySelector.GetPropertyAccessorBody((PropertyDeclarationSyntax)x, SyntaxKind.SetAccessorDeclaration)) },
				{ MemberKind.AddEventHandler, new Func<SyntaxNode, BlockSyntax>((SyntaxNode x) => MemberBodySelector.GetEventAccessorBody((EventDeclarationSyntax)x, SyntaxKind.AddAccessorDeclaration)) },
				{ MemberKind.RemoveEventHandler, new Func<SyntaxNode, BlockSyntax>((SyntaxNode x) => MemberBodySelector.GetEventAccessorBody((EventDeclarationSyntax)x, SyntaxKind.RemoveAccessorDeclaration)) }
			};
			if (memberKinds.TryGetValue(node.Kind, out func))
			{
				BlockSyntax syntaxNode = func(node.SyntaxNode);
				BlockSyntax blockSyntax = syntaxNode;
				if (syntaxNode != null)
				{
					return blockSyntax;
				}
			}
			return null;
		}

		private static BlockSyntax GetAccessorBody(AccessorListSyntax syntax, SyntaxKind kind)
		{
			AccessorDeclarationSyntax accessorDeclarationSyntax = syntax.Accessors.SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax a) => a.Kind() == kind);
			if (accessorDeclarationSyntax == null)
			{
				return null;
			}
			return accessorDeclarationSyntax.Body;
		}

		private static BlockSyntax GetEventAccessorBody(EventDeclarationSyntax syntax, SyntaxKind kind)
		{
			AccessorDeclarationSyntax accessorDeclarationSyntax = syntax.AccessorList.Accessors.SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax a) => a.Kind() == kind);
			if (accessorDeclarationSyntax == null)
			{
				return null;
			}
			return accessorDeclarationSyntax.Body;
		}

		private static BlockSyntax GetPropertyAccessorBody(PropertyDeclarationSyntax syntax, SyntaxKind kind)
		{
			AccessorDeclarationSyntax accessorDeclarationSyntax = syntax.AccessorList.Accessors.SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax a) => a.Kind() == kind);
			if (accessorDeclarationSyntax == null)
			{
				return null;
			}
			return accessorDeclarationSyntax.Body;
		}
	}
}