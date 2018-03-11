using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace RoslynMetrics.CSharp
{
	internal sealed class MemberNameResolver
	{
		private readonly SemanticModel semanticModel;

		public MemberNameResolver(SemanticModel semanticModel)
		{
			this.semanticModel = semanticModel;
		}

		private void AppendMethodIdentifier(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier = syntax.ExplicitInterfaceSpecifier;
			ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifierSyntax = explicitInterfaceSpecifier;
			if (explicitInterfaceSpecifier != null)
			{
				IdentifierNameSyntax name = explicitInterfaceSpecifierSyntax.Name as IdentifierNameSyntax;
				IdentifierNameSyntax identifierNameSyntax = name;
				if (name != null)
				{
					string valueText = identifierNameSyntax.Identifier.ValueText;
					builder.AppendFormat("{0}.", valueText);
				}
			}
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendMethodIdentifier(ConstructorDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendMethodIdentifier(DestructorDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendMethodIdentifier(PropertyDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendMethodIdentifier(EventDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append(syntax.Identifier.ValueText);
		}

		private void AppendParameters(BaseMethodDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append("(");
			ParameterListSyntax parameterList = syntax.ParameterList;
			if (parameterList != null)
			{
				SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;
				if (parameters.Any())
				{
					string str2 = string.Join(", ", ((IEnumerable<ParameterSyntax>)parameters).Select<ParameterSyntax, string>((ParameterSyntax x) => {
						StringBuilder stringBuilder = new StringBuilder();
						string str = string.Join(" ", (
							from m in (IEnumerable<SyntaxToken>)x.Modifiers
							select m.ValueText).ToArray<string>());
						if (!string.IsNullOrEmpty(str))
						{
							stringBuilder.Append(str);
							stringBuilder.Append(" ");
						}
						SemanticModel semanticModel = this.semanticModel;
						TypeSyntax type = x.Type;
						CancellationToken cancellationToken = new CancellationToken();
						ITypeSymbol symbol = ModelExtensions.GetSymbolInfo(semanticModel, (SyntaxNode)type, cancellationToken).Symbol as ITypeSymbol;
						ITypeSymbol typeSymbol = symbol;
						if (symbol == null)
						{
							return "?";
						}
						string str1 = MemberNameResolver.ResolveTypeName(typeSymbol);
						if (!string.IsNullOrWhiteSpace(str1))
						{
							stringBuilder.Append(str1);
						}
						return stringBuilder.ToString();
					}));
					builder.Append(str2);
				}
			}
			builder.Append(")");
		}

		private void AppendParameters(PropertyDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append("(");
			SemanticModel semanticModel = this.semanticModel;
			TypeSyntax type = syntax.Type;
			CancellationToken cancellationToken = new CancellationToken();
			SymbolInfo symbolInfo = ModelExtensions.GetSymbolInfo(semanticModel, (SyntaxNode)type, cancellationToken);
			ITypeSymbol symbol = symbolInfo.Symbol as ITypeSymbol;
			ITypeSymbol typeSymbol = symbol;
			if (symbol != null)
			{
				builder.Append(MemberNameResolver.ResolveTypeName(typeSymbol));
			}
			builder.Append(")");
		}

		private void AppendParameters(EventDeclarationSyntax syntax, StringBuilder builder)
		{
			builder.Append("(");
			SemanticModel semanticModel = this.semanticModel;
			TypeSyntax type = syntax.Type;
			CancellationToken cancellationToken = new CancellationToken();
			SymbolInfo symbolInfo = ModelExtensions.GetSymbolInfo(semanticModel, (SyntaxNode)type, cancellationToken);
			ITypeSymbol symbol = symbolInfo.Symbol as ITypeSymbol;
			ITypeSymbol typeSymbol = symbol;
			if (symbol != null)
			{
				builder.Append(MemberNameResolver.ResolveTypeName(typeSymbol));
			}
			builder.Append(")");
		}

		private void AppendReturnType(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			SemanticModel semanticModel = this.semanticModel;
			TypeSyntax returnType = syntax.ReturnType;
			CancellationToken cancellationToken = new CancellationToken();
			SymbolInfo symbolInfo = ModelExtensions.GetSymbolInfo(semanticModel, (SyntaxNode)returnType, cancellationToken);
			ITypeSymbol symbol = symbolInfo.Symbol as ITypeSymbol;
			ITypeSymbol typeSymbol = symbol;
			if (symbol != null)
			{
				builder.AppendFormat(" : {0}", MemberNameResolver.ResolveTypeName(typeSymbol));
			}
		}

		private void AppendReturnType(PropertyDeclarationSyntax syntax, StringBuilder builder)
		{
			SemanticModel semanticModel = this.semanticModel;
			TypeSyntax type = syntax.Type;
			CancellationToken cancellationToken = new CancellationToken();
			SymbolInfo symbolInfo = ModelExtensions.GetSymbolInfo(semanticModel, (SyntaxNode)type, cancellationToken);
			ITypeSymbol symbol = symbolInfo.Symbol as ITypeSymbol;
			ITypeSymbol typeSymbol = symbol;
			if (symbol != null)
			{
				builder.AppendFormat(" : {0}", MemberNameResolver.ResolveTypeName(typeSymbol));
			}
		}

		private static void AppendTypeParameters(MethodDeclarationSyntax syntax, StringBuilder builder)
		{
			if (syntax.TypeParameterList != null)
			{
				SeparatedSyntaxList<TypeParameterSyntax> parameters = syntax.TypeParameterList.Parameters;
				if (parameters.Any())
				{
					string str = string.Join(", ", 
						from x in (IEnumerable<TypeParameterSyntax>)parameters
						select x.Identifier.ValueText);
					builder.AppendFormat("<{0}>", str);
				}
			}
		}

		private string GetAddEventHandlerSignatureString(EventDeclarationSyntax syntax)
		{
			StringBuilder stringBuilder = new StringBuilder();
			((IEnumerable<AccessorDeclarationSyntax>)syntax.AccessorList.Accessors).SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax x) => x.Kind() == SyntaxKind.SetAccessorDeclaration);
			this.AppendMethodIdentifier(syntax, stringBuilder);
			stringBuilder.Append(".add");
			this.AppendParameters(syntax, stringBuilder);
			stringBuilder.Append(" : void");
			return stringBuilder.ToString();
		}

		public string GetConstructorSignatureString(ConstructorDeclarationSyntax syntax)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.AppendMethodIdentifier(syntax, stringBuilder);
			this.AppendParameters(syntax, stringBuilder);
			return stringBuilder.ToString();
		}

		private string GetDestructorSignatureString(DestructorDeclarationSyntax syntax)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.AppendMethodIdentifier(syntax, stringBuilder);
			return stringBuilder.ToString();
		}

		public string GetMethodSignatureString(MethodDeclarationSyntax syntax)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.AppendMethodIdentifier(syntax, stringBuilder);
			MemberNameResolver.AppendTypeParameters(syntax, stringBuilder);
			this.AppendParameters(syntax, stringBuilder);
			this.AppendReturnType(syntax, stringBuilder);
			return stringBuilder.ToString();
		}

		public string GetPropertyGetterSignatureString(PropertyDeclarationSyntax syntax)
		{
			StringBuilder stringBuilder = new StringBuilder();
			((IEnumerable<AccessorDeclarationSyntax>)syntax.AccessorList.Accessors).SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax x) => x.Kind() == SyntaxKind.GetAccessorDeclaration);
			this.AppendMethodIdentifier(syntax, stringBuilder);
			stringBuilder.Append(".get()");
			this.AppendReturnType(syntax, stringBuilder);
			return stringBuilder.ToString();
		}

		public string GetPropertySetterSignatureString(PropertyDeclarationSyntax syntax)
		{
			StringBuilder stringBuilder = new StringBuilder();
			((IEnumerable<AccessorDeclarationSyntax>)syntax.AccessorList.Accessors).SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax x) => x.Kind() == SyntaxKind.SetAccessorDeclaration);
			this.AppendMethodIdentifier(syntax, stringBuilder);
			stringBuilder.Append(".set");
			this.AppendParameters(syntax, stringBuilder);
			stringBuilder.Append(" : void");
			return stringBuilder.ToString();
		}

		private string GetRemoveEventHandlerSignatureString(EventDeclarationSyntax syntax)
		{
			StringBuilder stringBuilder = new StringBuilder();
			((IEnumerable<AccessorDeclarationSyntax>)syntax.AccessorList.Accessors).SingleOrDefault<AccessorDeclarationSyntax>((AccessorDeclarationSyntax x) => x.Kind() == SyntaxKind.SetAccessorDeclaration);
			this.AppendMethodIdentifier(syntax, stringBuilder);
			stringBuilder.Append(".remove");
			this.AppendParameters(syntax, stringBuilder);
			stringBuilder.Append(" : void");
			return stringBuilder.ToString();
		}

		private static string ResolveTypeName(ITypeSymbol symbol)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			IArrayTypeSymbol arrayTypeSymbol = symbol as IArrayTypeSymbol;
			IArrayTypeSymbol arrayTypeSymbol1 = arrayTypeSymbol;
			if (arrayTypeSymbol != null)
			{
				flag = true;
				symbol = arrayTypeSymbol1.ElementType;
			}
			stringBuilder.Append(symbol.Name);
			INamedTypeSymbol namedTypeSymbol = symbol as INamedTypeSymbol;
			INamedTypeSymbol namedTypeSymbol1 = namedTypeSymbol;
			if (namedTypeSymbol != null && namedTypeSymbol1.TypeArguments.Any<ITypeSymbol>())
			{
				IEnumerable<ITypeSymbol> typeSymbols = namedTypeSymbol1.TypeArguments.AsEnumerable<ITypeSymbol>();
				IEnumerable<string> array = typeSymbols.Select<ITypeSymbol, string>(new Func<ITypeSymbol, string>(MemberNameResolver.ResolveTypeName)).ToArray<string>();
				stringBuilder.AppendFormat("<{0}>", string.Join(", ", array));
			}
			if (flag)
			{
				stringBuilder.Append("[]");
			}
			return stringBuilder.ToString();
		}

		public bool TryResolveMemberSignatureString(MemberNode member, out string signature)
		{
			Func<SyntaxNode, string> func;
			signature = null;
			SyntaxNode syntaxNode = member.SyntaxNode;
			Dictionary<MemberKind, Func<SyntaxNode, string>> memberKinds = new Dictionary<MemberKind, Func<SyntaxNode, string>>()
			{
				{ MemberKind.Method, new Func<SyntaxNode, string>((SyntaxNode x) => this.GetMethodSignatureString((MethodDeclarationSyntax)x)) },
				{ MemberKind.Constructor, new Func<SyntaxNode, string>((SyntaxNode x) => this.GetConstructorSignatureString((ConstructorDeclarationSyntax)x)) },
				{ MemberKind.Destructor, new Func<SyntaxNode, string>((SyntaxNode x) => this.GetDestructorSignatureString((DestructorDeclarationSyntax)x)) },
				{ MemberKind.GetProperty, new Func<SyntaxNode, string>((SyntaxNode x) => this.GetPropertyGetterSignatureString((PropertyDeclarationSyntax)x)) },
				{ MemberKind.SetProperty, new Func<SyntaxNode, string>((SyntaxNode x) => this.GetPropertySetterSignatureString((PropertyDeclarationSyntax)x)) },
				{ MemberKind.AddEventHandler, new Func<SyntaxNode, string>((SyntaxNode x) => this.GetAddEventHandlerSignatureString((EventDeclarationSyntax)x)) },
				{ MemberKind.RemoveEventHandler, new Func<SyntaxNode, string>((SyntaxNode x) => this.GetRemoveEventHandlerSignatureString((EventDeclarationSyntax)x)) }
			};
			if (!memberKinds.TryGetValue(member.Kind, out func))
			{
				return false;
			}
			signature = func(syntaxNode);
			return !string.IsNullOrEmpty(signature);
		}
	}
}