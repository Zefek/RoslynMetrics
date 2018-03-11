using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics.Contracts
{
	public interface ILcomMetricsProvider
	{
		LcomMetrics Calculate(TypeDeclarationSyntax type);
	}
}