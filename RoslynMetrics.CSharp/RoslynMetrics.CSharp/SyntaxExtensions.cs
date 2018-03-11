using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RoslynMetrics.CSharp
{
	internal static class SyntaxExtensions
	{
		public static int GetLineNumber(this SyntaxNode syntax)
		{
			Location location = syntax.GetLocation();
			FileLinePositionSpan lineSpan = location.SourceTree.GetLineSpan(location.SourceSpan, new CancellationToken());
			return lineSpan.StartLinePosition.Line;
		}

		public static string GetName(this NamespaceDeclarationSyntax node, SyntaxNode rootNode)
		{
			NameSyntax name = node.Name;
			SourceText text = rootNode.GetText();
			return text.GetSubText(name.Span).ToString();
		}

		public static string GetName(this TypeDeclarationSyntax node, SyntaxNode rootNode)
		{
			SyntaxToken identifier = node.Identifier;
			SourceText text = rootNode.GetText();
			return text.GetSubText(identifier.Span).ToString();
		}
	}
}