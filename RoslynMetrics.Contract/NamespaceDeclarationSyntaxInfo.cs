using Microsoft.CodeAnalysis;
using System;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts
{
	public sealed class NamespaceDeclarationSyntaxInfo
	{
		public string CodeFile
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public SyntaxNode Syntax
		{
			get;
			set;
		}

		public NamespaceDeclarationSyntaxInfo()
		{
		}
	}
}