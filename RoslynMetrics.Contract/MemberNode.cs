using Microsoft.CodeAnalysis;
using System;
using System.Runtime.CompilerServices;

namespace RoslynMetrics.Contracts
{
	public class MemberNode
	{
		public string CodeFile
		{
			get;
			set;
		}

		public string DisplayName
		{
			get;
			set;
		}

		public MemberKind Kind
		{
			get;
			set;
		}

		public int LineNumber
		{
			get;
			set;
		}

		public Microsoft.CodeAnalysis.SyntaxNode SyntaxNode
		{
			get;
			set;
		}

		public MemberNode()
		{
		}
	}
}