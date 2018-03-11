using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoslynMetrics.CSharp
{
	internal sealed class TypeNameResolver
	{
		public TypeNameResolver()
		{
		}

		private static string GetContainingTypeName(ISymbol symbol)
		{
			INamedTypeSymbol namedTypeSymbol = symbol as INamedTypeSymbol;
			INamedTypeSymbol namedTypeSymbol1 = namedTypeSymbol;
			if (namedTypeSymbol == null)
			{
				return null;
			}
			string name = namedTypeSymbol1.Name;
			string containingTypeName = TypeNameResolver.GetContainingTypeName(namedTypeSymbol1.ContainingSymbol);
			if (string.IsNullOrWhiteSpace(containingTypeName))
			{
				return name;
			}
			return string.Concat(containingTypeName, ".", name);
		}

		private static string GetContainingTypeName(SyntaxNode syntax)
		{
			TypeDeclarationSyntax typeDeclarationSyntax = syntax as TypeDeclarationSyntax;
			TypeDeclarationSyntax typeDeclarationSyntax1 = typeDeclarationSyntax;
			if (typeDeclarationSyntax == null)
			{
				return null;
			}
			string valueText = typeDeclarationSyntax1.Identifier.ValueText;
			string containingTypeName = TypeNameResolver.GetContainingTypeName(typeDeclarationSyntax1.Parent);
			if (string.IsNullOrWhiteSpace(containingTypeName))
			{
				return valueText;
			}
			return string.Concat(containingTypeName, ".", valueText);
		}

		public static string GetName(TypeDeclarationSyntax syntax)
		{
			string valueText = syntax.Identifier.ValueText;
			string containingTypeName = TypeNameResolver.GetContainingTypeName(syntax.Parent);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				valueText = string.Concat(containingTypeName, ".", valueText);
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(valueText);
			if (syntax.TypeParameterList != null)
			{
				SeparatedSyntaxList<TypeParameterSyntax> parameters = syntax.TypeParameterList.Parameters;
				if (parameters.Any())
				{
					string str = string.Join(", ", 
						from x in (IEnumerable<TypeParameterSyntax>)parameters
						select x.Identifier.ValueText);
					stringBuilder.AppendFormat("<{0}>", str);
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetQualifiedName(ITypeSymbol symbol)
		{
			string name = symbol.Name;
			string containingTypeName = TypeNameResolver.GetContainingTypeName(symbol.ContainingSymbol);
			if (!string.IsNullOrWhiteSpace(containingTypeName))
			{
				name = string.Concat(containingTypeName, ".", name);
			}
			INamedTypeSymbol namedTypeSymbol = (INamedTypeSymbol)symbol;
			ImmutableArray<ITypeParameterSymbol>? nullable = null;
			if (new ImmutableArray<ITypeParameterSymbol>?(namedTypeSymbol.TypeParameters) != nullable && namedTypeSymbol.TypeParameters.Any<ITypeParameterSymbol>())
			{
				IEnumerable<string> array = (
					from x in namedTypeSymbol.TypeParameters.AsEnumerable<ITypeParameterSymbol>()
					select x.Name).ToArray<string>();
				string str = string.Join(", ", array);
				name = string.Concat(name, string.Format("<{0}>", str));
			}
			ISymbol containingSymbol = symbol.ContainingSymbol;
			while (containingSymbol != null && (int)containingSymbol.Kind == 12)
			{
				INamespaceSymbol namespaceSymbol = (INamespaceSymbol)containingSymbol;
				if (!namespaceSymbol.IsGlobalNamespace)
				{
					name = string.Concat(containingSymbol.Name, ".", name);
					containingSymbol = containingSymbol.ContainingSymbol;
				}
				else
				{
					name = string.Concat(name, string.Format(", {0}", namespaceSymbol.ContainingAssembly.Name));
					break;
				}
			}
			return name;
		}
	}
}