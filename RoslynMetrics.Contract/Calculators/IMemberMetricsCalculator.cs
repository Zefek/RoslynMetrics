using RoslynMetrics.Contracts.Metrics;

namespace RoslynMetrics.Contracts.Calculators
{
	public interface IMemberMetricsCalculator
	{
		MemberMetrics Calculate(TypeDeclarationSyntaxInfo typeNode);
	}
}