using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts
{
	public sealed class NamespaceDeclaration
	{
		public string Name
		{
			get;
			set;
		}

		public IEnumerable<NamespaceDeclarationSyntaxInfo> SyntaxNodes
		{
			get;
			set;
		}

		public NamespaceDeclaration()
		{
		}
	}
}