using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts
{
	public sealed class TypeDeclaration
	{
		public string Name
		{
			get;
			set;
		}

		public IEnumerable<TypeDeclarationSyntaxInfo> SyntaxNodes
		{
			get;
			set;
		}

		public TypeDeclaration()
		{
		}
	}
}