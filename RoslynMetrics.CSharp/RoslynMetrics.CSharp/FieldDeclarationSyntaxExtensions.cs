using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace RoslynMetrics.CSharp
{
	internal static class FieldDeclarationSyntaxExtensions
	{
		public static ISymbol GetDeclaredSymbol(this FieldDeclarationSyntax node, SemanticModel model)
		{
			ISymbol symbol = (
				from syntax in node.Declaration.Variables
				select ModelExtensions.GetDeclaredSymbol(model, syntax, new CancellationToken())).FirstOrDefault<ISymbol>();
			return symbol;
		}
	}
}