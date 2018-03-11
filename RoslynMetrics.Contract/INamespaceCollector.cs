using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RoslynMetrics.Contracts
{
	public interface INamespaceCollector
	{
		IEnumerable<T> GetNamespaces<T>(SyntaxNode commonNode)
		where T : SyntaxNode;
	}
}