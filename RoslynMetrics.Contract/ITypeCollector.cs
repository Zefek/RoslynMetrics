using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace RoslynMetrics.Contracts
{
	public interface ITypeCollector
	{
		IEnumerable<T> GetTypes<T>(SyntaxNode namespaceNode)
		where T : SyntaxNode;
	}
}